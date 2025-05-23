﻿using AutoMapper;
using FluentValidation;
using handover_api.Common;
using handover_api.Controllers.Dto;
using handover_api.Controllers.Request;
using handover_api.Controllers.Validator;
using handover_api.Models;
using handover_api.Service;
using handover_api.Service.ValueObject;
using handover_api.Utils;
using MaiBackend.Common.AutoMapper;
using MaiBackend.PublicApi.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using stock_api.Common.Utils;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using static handover_api.Service.ValueObject.CategoryComponent;

namespace handover_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(PermissionFilterAttribute))]
    public class HandoverController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<HandoverController> _logger;
        private readonly AuthHelpers _authHelpers;
        private readonly HandoverService _handoverService;
        private readonly MemberService _memberService;
        private readonly FileUploadService _fileUploadService;
        private readonly CreateHandoverDetailRequestValidator _createHandoverDetailRequestValidator;
        private readonly CreateHandoverDetailRequestV2Validator _createHandoverDetailRequestV2Validator;
        private readonly UpdateHandoverDetailRequestValidator _updateHandoverDetailRequestValidator;
        private readonly UpdateHandoverDetailRequestValidatorV2 _updateHandoverDetailRequestValidatorV2;
        private readonly AddDetailHandlersValidator _addDetailHandlersValidator;
        private readonly IValidator<GetUnReadRequest> _getUnReadRequestValidator;

        public HandoverController(IMapper mapper, ILogger<HandoverController> logger, AuthHelpers authHelpers, HandoverService handoverService, MemberService memberService, FileUploadService fileUploadService)
        {
            _mapper = mapper;
            _logger = logger;
            _authHelpers = authHelpers;
            _handoverService = handoverService;
            _memberService = memberService;
            _createHandoverDetailRequestValidator = new CreateHandoverDetailRequestValidator(ActionTypeEnum.Create, _memberService);
            _createHandoverDetailRequestV2Validator = new CreateHandoverDetailRequestV2Validator(ActionTypeEnum.Create, _memberService);
            _updateHandoverDetailRequestValidator = new UpdateHandoverDetailRequestValidator(_memberService);
            _updateHandoverDetailRequestValidatorV2 = new UpdateHandoverDetailRequestValidatorV2(_memberService);

            _addDetailHandlersValidator = new AddDetailHandlersValidator(_memberService);
            _fileUploadService = fileUploadService;
            _getUnReadRequestValidator = new GetUnReadRequestValidator();
        }


        [HttpPost("create")]
        [Authorize]
        public IActionResult CreateHandover(CreateHandoverDetailRequest createHandoverDetailRequest)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            Member creatorMember = memberAndPermissionSetting.Member;
            if (permissionSetting == null || !permissionSetting.IsCreateHandover)
            {
                return BadRequest(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }

            // 參數驗證
            var validationResult = _createHandoverDetailRequestValidator.Validate(createHandoverDetailRequest);
            if (!validationResult.IsValid)
            {
                return BadRequest(CommonResponse<dynamic>.BuildValidationFailedResponse(validationResult));
            }

            // 驗證是否同屬一個main
            var sheetRowIdList = createHandoverDetailRequest.RowDetails.Select(rd => rd.SheetRowId).ToList();
            var matchedSheetMainSettings = _handoverService.GetSheetMainListBySheetRowIdList(sheetRowIdList);
            var matchedSheetMainIdList = matchedSheetMainSettings.Select(main => main.SheetId).ToList();
            bool isTheSameMainId = matchedSheetMainSettings.Distinct().Count() == 1;
            if (!isTheSameMainId)
            {
                return BadRequest(new CommonResponse<dynamic>
                {
                    Result = false,
                    Message = "不可跨交班main setting"
                });
            }
            if (matchedSheetMainSettings[0].IsActive == null || matchedSheetMainSettings[0].IsActive == false)
            {
                return BadRequest(new CommonResponse<dynamic>
                {
                    Result = false,
                    Message = "此交班表設定為失效狀態"
                });
            }

            List<HandoverSheetRowWithGroup> neededSheetRowWithGroup = _handoverService.GetSheetRowsByMainSheetId(matchedSheetMainIdList[0]).Where(row => row.IsActive == true && row.IsGroupActive == true).ToList();
            var neededSheetRowCount = neededSheetRowWithGroup.Count;
            if (neededSheetRowCount != createHandoverDetailRequest.RowDetails.Count)
            {
                return BadRequest(new CommonResponse<dynamic>
                {
                    Result = false,
                    Message = $"交班單的row筆數:{createHandoverDetailRequest.RowDetails.Count}不對,需要{neededSheetRowCount}筆"
                });
            }

            List<Member> readerMemberList = _memberService.GetMembersByUserIdList(createHandoverDetailRequest.ReaderUserIds);

            if (readerMemberList.Find(m => m.UserId == creatorMember.UserId) == null)
            {
                readerMemberList.Add(creatorMember);
            }

            var createdJsonContent = _handoverService.CreateHandOverDetail(matchedSheetMainIdList[0], createHandoverDetailRequest.RowDetails, createHandoverDetailRequest.Title, createHandoverDetailRequest.Content, readerMemberList, creatorMember, createHandoverDetailRequest.FileAttIds);

            return Ok(new CommonResponse<string?>
            {
                Result = createdJsonContent != null,
                Data = createdJsonContent,
            });
        }

        [HttpPost("v2/create")]
        [Authorize]
        public IActionResult CreateHandoverV2(CreateHandoverDetailV2Request createHandoverDetailRequest)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            Member creatorMember = memberAndPermissionSetting.Member;
            if (permissionSetting == null || !permissionSetting.IsCreateHandover)
            {
                return BadRequest(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }

            // 參數驗證
            var validationResult = _createHandoverDetailRequestV2Validator.Validate(createHandoverDetailRequest);
            if (!validationResult.IsValid)
            {
                return BadRequest(CommonResponse<dynamic>.BuildValidationFailedResponse(validationResult));
            }


            List<Member> readerMemberList = _memberService.GetMembersByUserIdList(createHandoverDetailRequest.ReaderUserIds);

            if (readerMemberList.Find(m => m.UserId == creatorMember.UserId) == null)
            {
                readerMemberList.Add(creatorMember);
            }


            if (createHandoverDetailRequest.CategoryArray.Any())
            {

                // 驗證是否同屬一個main
                var categoryIdList = createHandoverDetailRequest.CategoryArray.Select(c => c.CategoryId).ToList();
                var (matchedSheetMainSettings, matchedGroupSheetSettings) = _handoverService.GetSheetMainListByCategoryIdList(categoryIdList);
                var matchedSheetMainIdList = matchedSheetMainSettings.Select(main => main.SheetId).ToList();
                var matchedSheetGroupIdList = matchedGroupSheetSettings.Select(group => group.SheetGroupId).ToList();
                bool isTheSameMainId = matchedSheetMainSettings.Distinct().Count() == 1;
                bool isTheSameGroupId = matchedSheetGroupIdList.Distinct().Count() == 1;
                if (!isTheSameMainId)
                {
                    return BadRequest(new CommonResponse<dynamic>
                    {
                        Result = false,
                        Message = "不可跨交班main setting"
                    });
                }
                if (matchedSheetMainSettings[0].IsActive == null || matchedSheetMainSettings[0].IsActive == false)
                {
                    return BadRequest(new CommonResponse<dynamic>
                    {
                        Result = false,
                        Message = "此交班表設定為失效狀態"
                    });
                }


                var createdJsonContent = _handoverService.CreateHandOverDetailV2(matchedSheetMainIdList[0], createHandoverDetailRequest.CategoryArray, createHandoverDetailRequest.Title, createHandoverDetailRequest.Content, readerMemberList, creatorMember, createHandoverDetailRequest.FileAttIds);

                return Ok(new CommonResponse<string?>
                {
                    Result = createdJsonContent != null,
                    Data = createdJsonContent,
                });
            }
            return BadRequest(new CommonResponse<string?>
            {
                Result = false,
                Message = "categoryArray不可為空"
            });
            
        }

        [HttpPost("update")]
        [Authorize]
        public IActionResult UpdateHandover(UpdateHandoverDetailRequest updateHandoverDetailRequest)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            Member creatorMember = memberAndPermissionSetting.Member;
            if (permissionSetting == null || !permissionSetting.IsUpdateHandover)
            {
                return BadRequest(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }

            // 參數驗證
            var validationResult = _updateHandoverDetailRequestValidator.Validate(updateHandoverDetailRequest);
            if (!validationResult.IsValid)
            {
                return BadRequest(CommonResponse<dynamic>.BuildValidationFailedResponse(validationResult));
            }
            var handoverDetail = _handoverService.GetHandoverDetail(updateHandoverDetailRequest.HandoverDetailId);
            if (handoverDetail == null)
            {
                return BadRequest(new CommonResponse<dynamic>
                {
                    Result = false,
                    Message = "此交班表不存在",
                });
            }

            List<Member> readerMemberList = _memberService.GetMembersByUserIdList(updateHandoverDetailRequest.ReaderUserIds);

            var updatedJsonContent = _handoverService.UpdateHandover(handoverDetail, updateHandoverDetailRequest.RowDetails, updateHandoverDetailRequest.Title,
                updateHandoverDetailRequest.Content, readerMemberList, updateHandoverDetailRequest.FileAttIds);

            return Ok(new CommonResponse<string?>
            {
                Result = updatedJsonContent != null,
                Data = updatedJsonContent,
            });
        }

        [HttpPost("v2/update")]
        [Authorize]
        public IActionResult UpdateHandoverV2(UpdateHandoverDetailRequestV2 updateHandoverDetailRequest)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            Member creatorMember = memberAndPermissionSetting.Member;
            if (permissionSetting == null || !permissionSetting.IsUpdateHandover)
            {
                return BadRequest(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }

            // 參數驗證
            var validationResult = _updateHandoverDetailRequestValidatorV2.Validate(updateHandoverDetailRequest);
            if (!validationResult.IsValid)
            {
                return BadRequest(CommonResponse<dynamic>.BuildValidationFailedResponse(validationResult));
            }
            var handoverDetail = _handoverService.GetHandoverDetail(updateHandoverDetailRequest.HandoverDetailId);
            if (handoverDetail == null)
            {
                return BadRequest(new CommonResponse<dynamic>
                {
                    Result = false,
                    Message = "此交班表不存在",
                });
            }

            List<Member> readerMemberList = _memberService.GetMembersByUserIdList(updateHandoverDetailRequest.ReaderUserIds);

            var updatedJsonContent = _handoverService.UpdateHandoverV2(handoverDetail, updateHandoverDetailRequest.CategoryArray, updateHandoverDetailRequest.Title,
                updateHandoverDetailRequest.Content, readerMemberList, updateHandoverDetailRequest.FileAttIds);

            return Ok(new CommonResponse<string?>
            {
                Result = updatedJsonContent != null,
                Data = updatedJsonContent,
            });
        }


        [HttpDelete("{handoverDetailId}")]
        [Authorize]
        public IActionResult InactiveHandoverDetail(string handoverDetailId)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            if (permissionSetting == null || !permissionSetting.IsUpdateHandover)
            {
                return BadRequest(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }


            var handoverDetail = _handoverService.GetHandoverDetail(handoverDetailId);
            if (handoverDetail == null)
            {
                return BadRequest(new CommonResponse<dynamic>
                {
                    Result = false,
                    Message = "此交班表不存在",
                });
            }

            var result = _handoverService.InActiveHandoverDetail(handoverDetail);

            return Ok(new CommonResponse<dynamic>
            {
                Result = result,
            });
        }


        [HttpPost("search")]
        [Authorize]
        public IActionResult SearchHandoverDetails(SearchHandoverDetailRequest searchHandoverDetailRequest)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            Member loginMember = memberAndPermissionSetting.Member;

            if ((searchHandoverDetailRequest.StartDate != null && !Regex.IsMatch(searchHandoverDetailRequest.StartDate, @"^\d{3}/\d{2}/\d{2}$"))
                || (searchHandoverDetailRequest.EndDate != null && !Regex.IsMatch(searchHandoverDetailRequest.EndDate, @"^\d{3}/\d{2}/\d{2}$")))
            {
                return BadRequest(new CommonResponse<dynamic>
                {
                    Result = false,
                    Message = "時間格式必需為yyy/mm/dd",
                });
            }
            var startDate = searchHandoverDetailRequest.StartDate != null ? APIMappingProfile.ParseDateString(searchHandoverDetailRequest.StartDate) : null;
            var endDate = searchHandoverDetailRequest.EndDate != null ? APIMappingProfile.ParseDateString(searchHandoverDetailRequest.EndDate) : null;
            endDate = endDate?.AddDays(1);
            var (handoverDetailList,totalPages) = _handoverService.SearchHandoverDetails(searchHandoverDetailRequest.MainSheetId, searchHandoverDetailRequest.StartDate, searchHandoverDetailRequest.EndDate,
                searchHandoverDetailRequest.PaginationCondition, searchHandoverDetailRequest.SearchString);

            List<HandoverDetailWithReadDto> handoverDetailWithReadDtoList = _mapper.Map<List<HandoverDetailWithReadDto>>(handoverDetailList);

            var handoverReaderList = _handoverService.GetHandoverDetailReadersByUserId(loginMember.UserId);

            List<string> fileAttIdsList = handoverDetailList.Select(hd => hd.FileAttIds).ToList();
            HashSet<string> allDistinctfileAttIds = new();
            fileAttIdsList.ForEach(fileAttIdsString =>
            {
                var fileAttIdsList = fileAttIdsString.Split(",");
                foreach (var fileAttId in fileAttIdsList)
                {
                    allDistinctfileAttIds.Add(fileAttId);
                }
            });
            List<FileDetailInfo> fileDetailInfos = _handoverService.GetFileDetailInfos(allDistinctfileAttIds.ToList());



            handoverDetailWithReadDtoList.ForEach(dto =>
            {
                var matchedReader = handoverReaderList.Find(reader => reader.HandoverDetailId == dto.HandoverDetailId);
                if (matchedReader != null)
                {
                    dto.IsRead = matchedReader.IsRead;
                }
                else
                {
                    // 如果不在 readUser 名單內，視為已讀
                    dto.IsRead = true;
                }
                var fileAttIdList = dto.FileAttIds.Split(",");
                List<FileDetailInfo> matchedFiles = fileDetailInfos.Where(fdi => fileAttIdList.Contains(fdi.AttId)).ToList();
                dto.Files = matchedFiles;
            });


            return Ok(new CommonResponse<List<HandoverDetailWithReadDto>>
            {
                Result = true,
                Data = handoverDetailWithReadDtoList
            });
        }

        [HttpPost("v2/search")]
        [Authorize]
        public IActionResult SearchHandoverDetailsV2(SearchHandoverDetailRequest searchHandoverDetailRequest)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            Member loginMember = memberAndPermissionSetting.Member;

            if ((searchHandoverDetailRequest.StartDate != null && DateTimeHelper.ParseDateString(searchHandoverDetailRequest.StartDate)==null)
                || (searchHandoverDetailRequest.EndDate != null && DateTimeHelper.ParseDateString(searchHandoverDetailRequest.EndDate) == null))
            {
                return BadRequest(new CommonResponse<dynamic>
                {
                    Result = false,
                    Message = "時間格式必需為yyyy/MM/dd",
                });
            }
            
            var (handoverDetailList,totalPages) = _handoverService.SearchHandoverDetails(searchHandoverDetailRequest.MainSheetId, searchHandoverDetailRequest.StartDate, searchHandoverDetailRequest.EndDate,
                searchHandoverDetailRequest.PaginationCondition, searchHandoverDetailRequest.SearchString);

            List<HandoverDetailWithReadV2Dto> handoverDetailWithReadDtoList = _mapper.Map<List<HandoverDetailWithReadV2Dto>>(handoverDetailList);

            var handoverReaderList = _handoverService.GetHandoverDetailReadersByUserId(loginMember.UserId);

            List<string> fileAttIdsList = handoverDetailList.Select(hd => hd.FileAttIds).ToList();
            HashSet<string> allDistinctfileAttIds = new();
            fileAttIdsList.ForEach(fileAttIdsString =>
            {
                var fileAttIdsList = fileAttIdsString.Split(",");
                foreach (var fileAttId in fileAttIdsList)
                {
                    allDistinctfileAttIds.Add(fileAttId);
                }
            });
            List<FileDetailInfo> fileDetailInfos = _handoverService.GetFileDetailInfos(allDistinctfileAttIds.ToList());



            handoverDetailWithReadDtoList.ForEach(dto =>
            {
                var matchedReader = handoverReaderList.Find(reader => reader.HandoverDetailId == dto.HandoverDetailId);
                if (matchedReader != null)
                {
                    dto.IsRead = matchedReader.IsRead;
                }
                else
                {
                    // 如果不在 readUser 名單內，視為已讀
                    dto.IsRead = true;
                }
                var fileAttIdList = dto.FileAttIds.Split(",");
                List<FileDetailInfo> matchedFiles = fileDetailInfos.Where(fdi => fileAttIdList.Contains(fdi.AttId)).ToList();
                dto.Files = matchedFiles;

                dto.CategoryArray = JsonConvert.DeserializeObject<List<CategoryComponent>>(dto.JsonContent);
                dto.CategoryArray = dto.CategoryArray.OrderBy(c => c.CategoryRank)
                        .ThenBy(c => c.GroupRank)
                        .ToList();
            });

            var mainSheetIds = handoverDetailWithReadDtoList.Select(d=>d.MainSheetId).Distinct().ToList();
            var mainSheets = _handoverService.GetHandoverMainSettings(mainSheetIds);

            handoverDetailWithReadDtoList.ForEach(e =>
            {
                var matchedMain = mainSheets.Where(m => m.SheetId == e.MainSheetId).FirstOrDefault();
                e.MainSheetTitle = matchedMain.Title;
            });


            return Ok(new CommonResponse<List<HandoverDetailWithReadV2Dto>>
            {
                Result = true,
                Data = handoverDetailWithReadDtoList,
                TotalPages = totalPages 
            });
        }

        [HttpGet("detail/{handoverDetailId}")]
        [Authorize]
        public IActionResult ReadHandoverDetail(string handoverDetailId)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            Member reader = memberAndPermissionSetting.Member;
            var handoverDetail = _handoverService.GetHandoverDetailByDetailId(handoverDetailId);
            if (handoverDetail == null)
            {
                return BadRequest(new CommonResponse<HandoverDetail>
                {
                    Result = false,
                    Message = "此交班表不存在",
                });
            }
            HandoverDetailWithReaders handoverDetailWithReaders = _mapper.Map<HandoverDetailWithReaders>(handoverDetail);

            if (!string.IsNullOrEmpty(handoverDetail.FileAttIds))
            {
                List<string> fileAttIds = handoverDetail.FileAttIds.Split(",").ToList();
                List<FileDetailInfo> fileDetailInfos = _handoverService.GetFileDetailInfos(fileAttIds);
                handoverDetailWithReaders.Files = fileDetailInfos;
            }

            var result = _handoverService.ReadHandoverDetail(handoverDetailId, reader.UserId);

            var handoverReaders = _handoverService.GetHandoverDetailReadersByDetailId(handoverDetailId);
            var handoverReaderDtoList = _mapper.Map<List<HandoverDetailReaderDto>>(handoverReaders);
            var readersMemberInto = _memberService.GetActiveMembersByUserIds(handoverReaders.Select(hr => hr.UserId).ToList());
            handoverReaderDtoList.ForEach(dto =>
            {
                var matchedReaderMember = readersMemberInto.Find(m => m.UserId == dto.UserId);
                dto.PhotoUrl = matchedReaderMember?.PhotoUrl;
            });

            handoverDetailWithReaders.HandoverDetailReader = handoverReaderDtoList;
            var handoverDetailHandlers = _handoverService.GetHandoverDetailHandlersById(handoverDetailId);
            handoverDetailWithReaders.HandoverDetailHandlers = handoverDetailHandlers;


            return Ok(new CommonResponse<HandoverDetailWithReaders>
            {
                Result = result,
                Data = handoverDetailWithReaders
            });
        }

        [HttpGet("v2/detail/{handoverDetailId}")]
        [Authorize]
        public IActionResult GetHandoverDetailV2(string handoverDetailId)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            Member reader = memberAndPermissionSetting.Member;
            var handoverDetail = _handoverService.GetHandoverDetailByDetailId(handoverDetailId);
            List<int> groupSettingIds = new();
            if (handoverDetail == null)
            {
                return BadRequest(new CommonResponse<HandoverDetail>
                {
                    Result = false,
                    Message = "此交班表不存在",
                });
            }
            SheetSettingAndDetail sheetSettingAndDetail = _mapper.Map<SheetSettingAndDetail>(handoverDetail);

            if (!string.IsNullOrEmpty(handoverDetail.FileAttIds))
            {
                List<string> fileAttIds = handoverDetail.FileAttIds.Split(",").ToList();
                List<FileDetailInfo> fileDetailInfos = _handoverService.GetFileDetailInfos(fileAttIds);
                sheetSettingAndDetail.Files = fileDetailInfos;
            }

            //var result = _handoverService.ReadHandoverDetail(handoverDetailId, reader.UserId);

            var handoverReaders = _handoverService.GetHandoverDetailReadersByDetailId(handoverDetailId);
            var handoverReaderDtoList = _mapper.Map<List<HandoverDetailReaderDto>>(handoverReaders);
            var readersMemberInto = _memberService.GetActiveMembersByUserIds(handoverReaders.Select(hr => hr.UserId).ToList());
            handoverReaderDtoList.ForEach(dto =>
            {
                var matchedReaderMember = readersMemberInto.Find(m => m.UserId == dto.UserId);
                dto.PhotoUrl = matchedReaderMember?.PhotoUrl;

            });

            sheetSettingAndDetail.HandoverDetailReader = handoverReaderDtoList;
            var categoryArray = JsonConvert.DeserializeObject<List<CategoryComponent>>(handoverDetail.JsonContent).OrderByDescending(c=>c.CategoryRank).ToList();
            var distinctGroupIdList = categoryArray.Select(c=>c.SheetGroupId).Distinct().ToList();

            distinctGroupIdList.ForEach(groupId =>
            {
                var groupWithCategoryArrayDto = new GroupWithCategoryArrayDto()
                {
                    SheetGroupId = groupId.Value
                };
                var matchedCategoryArray = categoryArray.Where(c => c.SheetGroupId == groupId).ToList();
                groupWithCategoryArrayDto.GroupTitle = matchedCategoryArray[0].GroupTitle;
                groupWithCategoryArrayDto.GroupRank = matchedCategoryArray[0].GroupRank.Value;
                groupWithCategoryArrayDto.CategoryArray = matchedCategoryArray;
                sheetSettingAndDetail.HandoverSheetGroupList.Add(groupWithCategoryArrayDto);
            });

            sheetSettingAndDetail.HandoverSheetGroupList = sheetSettingAndDetail.HandoverSheetGroupList.OrderByDescending(g=>g.GroupRank).ToList();
            var handoverDetailHandlers = _handoverService.GetHandoverDetailHandlersById(handoverDetailId);
            sheetSettingAndDetail.HandoverDetailHandlers = handoverDetailHandlers;


            return Ok(new CommonResponse<SheetSettingAndDetail>
            {
                Result = true,
                Data = sheetSettingAndDetail
            });
        }

        [HttpGet("detail/my")]
        [Authorize]
        public IActionResult GetMyHandoverDetail()
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            Member reader = memberAndPermissionSetting.Member;
            var handoverDetailDtoList = _handoverService.GetMyHandoverDetailDtoList(reader.UserId);
            var handoverDetailHandlers = _handoverService.GetHandoverDetailHandlersByIds(handoverDetailDtoList.Select(d => d.HandoverDetailId).ToList());
            foreach (var handoverDetailDto in handoverDetailDtoList)
            {
                if (!string.IsNullOrEmpty(handoverDetailDto.FileAttIds))
                {
                    List<string> fileAttIds = handoverDetailDto.FileAttIds.Split(",").ToList();
                    List<FileDetailInfo> fileDetailInfos = _handoverService.GetFileDetailInfos(fileAttIds);
                    handoverDetailDto.Files = fileDetailInfos;
                }
                var matchedHandoverDetailHandlers = handoverDetailHandlers.Where(h => h.HandoverDetailId == handoverDetailDto.HandoverDetailId).ToList();
                handoverDetailDto.HandoverDetailHandlers = matchedHandoverDetailHandlers;
            }
            return Ok(new CommonResponse<List<MyHandoverDetailDto>>
            {
                Result = true,
                Data = handoverDetailDtoList
            });
        }

        [HttpGet("v2/detail/my")]
        [Authorize]
        public IActionResult GetMyHandoverDetailV2()
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            Member reader = memberAndPermissionSetting.Member;
            var handoverDetailDtoList = _handoverService.GetMyHandoverDetailDtoListV2(reader.UserId);
            var handoverDetailHandlers = _handoverService.GetHandoverDetailHandlersByIds(handoverDetailDtoList.Select(d=>d.HandoverDetailId).ToList());
            foreach (var handoverDetailDto in handoverDetailDtoList)
            {
                if (!string.IsNullOrEmpty(handoverDetailDto.FileAttIds))
                {
                    List<string> fileAttIds = handoverDetailDto.FileAttIds.Split(",").ToList();
                    List<FileDetailInfo> fileDetailInfos = _handoverService.GetFileDetailInfos(fileAttIds);
                    handoverDetailDto.Files = fileDetailInfos;
                }
                var matchedHandoverDetailHandlers = handoverDetailHandlers.Where(h => h.HandoverDetailId == handoverDetailDto.HandoverDetailId).ToList();
                handoverDetailDto.HandoverDetailHandlers = matchedHandoverDetailHandlers;
            }
            return Ok(new CommonResponse<List<MyHandoverDetailV2Dto>>
            {
                Result = true,
                Data = handoverDetailDtoList
            });
        }

        [HttpGet("histories/{handoverDetailId}")]
        [Authorize]
        public IActionResult GetHandoverDetailHistories(string handoverDetailId)
        {

            var handoverDetailHistories = _handoverService.GetHandoverDetailHistories(handoverDetailId);

            return Ok(new CommonResponse<List<HandoverDetailHistory>>
            {
                Result = true,
                Data = handoverDetailHistories
            });
        }


        [HttpGet("v2/histories/{handoverDetailId}")]
        [Authorize]
        public IActionResult GetHandoverDetailHistoriesV2(string handoverDetailId)
        {

            var handoverDetailHistories = _handoverService.GetHandoverDetailHistories(handoverDetailId);

            var handoverDetailHistoryDtoList = _mapper.Map<List<HandoverDetailHistoryDto>>(handoverDetailHistories);
            handoverDetailHistoryDtoList.ForEach(dto =>
            {
                if (dto.OldJsonContent != null)
                {
                    dto.OldCategoryArray = JsonConvert.DeserializeObject<List<CategoryComponent>>(dto.OldJsonContent);
                }
                if (dto.NewJsonContent != null)
                {
                    dto.NewCategoryArray = JsonConvert.DeserializeObject<List<CategoryComponent>>(dto.NewJsonContent);
                }

            });


            return Ok(new CommonResponse<List<HandoverDetailHistoryDto>>
            {
                Result = true,
                Data = handoverDetailHistoryDtoList
            });
        }


        [HttpPost("Files/upload")]
        [Authorize]
        public async Task<IActionResult> UploadFile([FromForm] UploadFilesRequest uploadFilesRequest)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;


            var fileDetails = await _fileUploadService.PostFilesAsync(uploadFilesRequest.Files, new List<string> { "handover" });
            var fileDetailInfos = _mapper.Map<List<FileDetailInfo>>(fileDetails);
            bool result = _fileUploadService.AddFileDetailInfo(fileDetailInfos);

            return Ok(new CommonResponse<List<FileDetailInfo>>
            {
                Result = result,
                Message = result ? "" : "上傳失敗",
                Data = fileDetailInfos
            });
        }
        [HttpGet("Files/{attid}")]
        public async Task<IActionResult> DownloadFile(string attid)
        {
            var fileDetail = _fileUploadService.GetFileDetail(attid);
            if (fileDetail == null)
            {
                return NotFound();
            }
            var fileStream = _fileUploadService.Download(fileDetail);
            return fileStream;
        }

        [HttpPost("read")]
        [Authorize]
        public IActionResult ReadDetail(ReadDetailRequest request)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            Member loginMember = memberAndPermissionSetting.Member;

            var detail = _handoverService.GetHandoverDetailByDetailId(request.HandoverDetailId);
            if (detail == null)
            {
                return BadRequest(new CommonResponse<dynamic>
                {
                    Result = false,
                    Message = "該交班表不存在"
                });
            }

            _handoverService.ReadDetail(request.HandoverDetailId, loginMember.UserId);

            return Ok(new CommonResponse<List<HandoverDetailWithReadDto>>
            {
                Result = true,
            });
        }

        [HttpPost("addDetailHandlers")]
        [Authorize]
        public IActionResult AddDetailHandlers(AddDetailHandlersRequest request)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            Member loginMember = memberAndPermissionSetting.Member;

            var validationResult = _addDetailHandlersValidator.Validate(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(CommonResponse<dynamic>.BuildValidationFailedResponse(validationResult));
            }

            var detail = _handoverService.GetHandoverDetailByDetailId(request.handoverDetailId);
            if (detail == null)
            {
                return BadRequest(new CommonResponse<dynamic>
                {
                    Result = false,
                    Message = "該交班表不存在"
                });
            }


            _handoverService.AddDetailHandlers(request.UserList, request.handoverDetailId);

            return Ok(new CommonResponse<List<HandoverDetailWithReadDto>>
            {
                Result = true,
            });
        }

        [HttpPost("getUnRead")]
        [Authorize]
        public IActionResult GetUnRead(GetUnReadRequest request)
        {
            var loginMemberAndPermission = _authHelpers.GetMemberAndPermissionSetting(User);
            var userId = loginMemberAndPermission!.Member.UserId;


            var validationResult = _getUnReadRequestValidator.Validate(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(CommonResponse<dynamic>.BuildValidationFailedResponse(validationResult));
            }

            var data = _handoverService.GetUnReadDataList(request);

            return Ok(new CommonResponse<List<HandoverUnReadDto>>
            {
                Result = true,
                Data = data
            });
        }
    }
}
