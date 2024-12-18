﻿using AutoMapper;
using FluentValidation;
using handover_api.Common;
using handover_api.Controllers.Request;
using handover_api.Controllers.Validator;
using handover_api.Models;
using handover_api.Service;
using handover_api.Service.ValueObject;
using handover_api.Utils;
using MaiBackend.PublicApi.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace handover_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(PermissionFilterAttribute))]
    public class SheetSettingController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<SheetSettingController> _logger;
        private readonly AuthHelpers _authHelpers;
        private readonly HandoverService _handoverService;
        private readonly IValidator<CreateOrUpdateSheetSettingMainRequest> _createSheetSettingMainRequestValidator;
        private readonly IValidator<CreateOrUpdateSheetSettingMainRequest> _updateSheetSettingMainRequestValidator;
        private readonly IValidator<CreateOrUpdateSheetSettingGroupRequest> _createSheetSettingGroupRequestValidator;
        private readonly IValidator<CreateOrUpdateSheetSettingGroupRequest> _updateSheetSettingGroupRequestValidator;
        private readonly IValidator<CreateOrUpdateSheetSettingRowRequest> _createSheetSettingRowRequestValidator;
        private readonly IValidator<CreateOrUpdateSheetSettingRowRequest> _updateSheetSettingRowRequestValidator;
        private readonly FileUploadService _fileUploadService;

        public SheetSettingController(IMapper mapper, ILogger<SheetSettingController> logger, AuthHelpers authHelpers, HandoverService handoverService, FileUploadService fileUploadService)
        {
            _mapper = mapper;
            _logger = logger;
            _authHelpers = authHelpers;
            _handoverService = handoverService;
            _createSheetSettingMainRequestValidator = new CreateOrUpdateSheetSettingMainRequestValidator(ActionTypeEnum.Create);
            _updateSheetSettingMainRequestValidator = new CreateOrUpdateSheetSettingMainRequestValidator(ActionTypeEnum.Update);
            _createSheetSettingGroupRequestValidator = new CreateOrUpdateSheetSettingGroupRequestValidator(ActionTypeEnum.Create,handoverService);
            _updateSheetSettingGroupRequestValidator = new CreateOrUpdateSheetSettingGroupRequestValidator(ActionTypeEnum.Update, handoverService);
            _createSheetSettingRowRequestValidator = new CreateOrUpdateSheetSettingRowRequestValidator(ActionTypeEnum.Create, handoverService);
            _updateSheetSettingRowRequestValidator = new CreateOrUpdateSheetSettingRowRequestValidator(ActionTypeEnum.Update, handoverService);
            _fileUploadService = fileUploadService;
        }

        [HttpGet("list")]
        [Authorize]
        public IActionResult List()
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;


            var data = _handoverService.GetAllSettings();
            var response = new CommonResponse<List<SheetSetting>>()
            {
                Result = true,
                Message = "",
                Data = data
            };
            return Ok(response);
        }

        [HttpGet("v2/list")]
        [Authorize]
        public IActionResult ListV2()
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;


            var data = _handoverService.GetAllSettingsV2();
            var response = new CommonResponse<List<SheetSettingV2>>()
            {
                Result = true,
                Message = "",
                Data = data
            };
            return Ok(response);
        }


        [HttpPost("Images/upload")]
        [Authorize]
        public async Task<IActionResult> UploadImage([FromForm] UploadFilesRequest uploadFilesRequest)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            if (permissionSetting == null || (!permissionSetting.IsCreateHandover && !permissionSetting.IsUpdateHandover))
            {
                return Unauthorized(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }

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
        [HttpGet("Images/{attid}")]
        public async Task<IActionResult> DownloadImage(string attid)
        {
            var fileDetail = _fileUploadService.GetFileDetail(attid);
            if (fileDetail == null)
            {
                return NotFound();
            }
            var fileStream = _fileUploadService.Download(fileDetail);
            return fileStream;
        }

        [HttpPost("create")]
        [Authorize]
        public IActionResult CreateSettingMain(CreateOrUpdateSheetSettingMainRequest createSettingMainRequest)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            if (memberAndPermissionSetting == null || memberAndPermissionSetting.Member == null || permissionSetting == null || !permissionSetting.IsCreateHandover)
            {
                return Unauthorized(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }
            var validationResult = _createSheetSettingMainRequestValidator.Validate(createSettingMainRequest);

            if (!validationResult.IsValid)
            {
                return BadRequest(CommonResponse<dynamic>.BuildValidationFailedResponse(validationResult));
            }


            var newSettingMain = _mapper.Map<HandoverSheetMain>(createSettingMainRequest);
            newSettingMain.CreatorName = memberAndPermissionSetting.Member.DisplayName;
            var result = _handoverService.CreateHandoverSheetMain(newSettingMain);
            var response = new CommonResponse<HandoverSheetMain>()
            {
                Result = result,
                Message = "",
                Data = null
            };
            return Ok(response);
        }

        [HttpPost("update")]
        [Authorize]
        public IActionResult UpdateSettingMain(List<CreateOrUpdateSheetSettingMainRequest> updateSettingMainRequestList)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            if (permissionSetting == null || !permissionSetting.IsUpdateHandover)
            {
                return Unauthorized(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }

            foreach (var updateSettingMainRequest in updateSettingMainRequestList)
            {
                var validationResult = _updateSheetSettingMainRequestValidator.Validate(updateSettingMainRequest);

                if (!validationResult.IsValid)
                {
                    return BadRequest(CommonResponse<dynamic>.BuildValidationFailedResponse(validationResult));
                }
            }

            var updateSettingMainList = _mapper.Map<List<HandoverSheetMain>>(updateSettingMainRequestList);
            var data = _handoverService.UpdateHandoverSheetMains(updateSettingMainList);
            var response = new CommonResponse<List<HandoverSheetMain>>()
            {
                Result = true,
                Message = "",
                Data = data
            };
            return Ok(response);
        }

        [HttpDelete("delete/{sheetId}")]
        [Authorize]
        public IActionResult Delete(int sheetId)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            if (memberAndPermissionSetting == null || permissionSetting == null || !permissionSetting.IsDeleteHandover)
            {
                return Unauthorized(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }
            _handoverService.InActiveHandoverSheetMain(sheetId);

            var response = new CommonResponse<dynamic>()
            {
                Result = true,
                Message = "",
                Data = null
            };
            return Ok(response);
        }

        [HttpPost("Group/create")]
        [Authorize]
        public IActionResult CreateSettingGroup(CreateOrUpdateSheetSettingGroupRequest createSettingGroupRequest)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            if (memberAndPermissionSetting == null || memberAndPermissionSetting.Member == null || permissionSetting == null || !permissionSetting.IsCreateHandover)
            {
                return Unauthorized(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }
            var validationResult = _createSheetSettingGroupRequestValidator.Validate(createSettingGroupRequest);

            if (!validationResult.IsValid)
            {
                return BadRequest(CommonResponse<dynamic>.BuildValidationFailedResponse(validationResult));
            }


            var newSettingGroup = _mapper.Map<HandoverSheetGroup>(createSettingGroupRequest);
            newSettingGroup.CreatorName = memberAndPermissionSetting.Member.DisplayName;
            var result = _handoverService.CreateHandoverSheetGroup(newSettingGroup);
            var response = new CommonResponse<HandoverSheetGroup>()
            {
                Result = result,
                Message = "",
                Data = null
            };
            return Ok(response);
        }

        [HttpPost("Group/update")]
        [Authorize]
        public IActionResult UpdateSettingGroup(CreateOrUpdateSheetSettingGroupRequest updateSettingGroupRequest)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            if (permissionSetting == null || !permissionSetting.IsUpdateHandover)
            {
                return Unauthorized(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }
           
            var validationResult = _updateSheetSettingGroupRequestValidator.Validate(updateSettingGroupRequest);

            if (!validationResult.IsValid)
            {
                return BadRequest(CommonResponse<dynamic>.BuildValidationFailedResponse(validationResult));
            }

            var updateSettingGroup = _mapper.Map<HandoverSheetGroup>(updateSettingGroupRequest);
            var data = _handoverService.UpdateHandoverSheetGroups(new List<HandoverSheetGroup>() { updateSettingGroup });
            var response = new CommonResponse<HandoverSheetGroup>()
            {
                Result = true,
                Message = "",
                Data = data[0]
            };
            return Ok(response);
        }

        [HttpDelete("Group/delete/{sheetGroupId}")]
        [Authorize]
        public IActionResult DeleteGroup(int sheetGroupId)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            if (memberAndPermissionSetting == null || permissionSetting == null || !permissionSetting.IsDeleteHandover)
            {
                return Unauthorized(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }
            _handoverService.InActiveHandoverSheetGroup(sheetGroupId);

            var response = new CommonResponse<dynamic>()
            {
                Result = true,
                Message = "",
                Data = null
            };
            return Ok(response);
        }

        [HttpPost("Row/create")]
        [Authorize]
        public IActionResult CreateSettingRow(CreateOrUpdateSheetSettingRowRequest createSettingRowRequest)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            if (memberAndPermissionSetting == null || memberAndPermissionSetting.Member == null || permissionSetting == null || !permissionSetting.IsCreateHandover)
            {
                return Unauthorized(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }
            var validationResult = _createSheetSettingRowRequestValidator.Validate(createSettingRowRequest);

            if (!validationResult.IsValid)
            {
                return BadRequest(CommonResponse<dynamic>.BuildValidationFailedResponse(validationResult));
            }
            var mainSheetId = createSettingRowRequest.MainSheetId;
            var sheetGroupId = createSettingRowRequest.SheetGroupId;
            var sheetMain = _handoverService.GetSheetMainByMainSheetId(mainSheetId.Value);
            if (sheetMain == null)
            {
                return BadRequest(new CommonResponse<dynamic> { Result=false,Message= "此mainSheetId不存在" });
            }
            var sheetGroup = _handoverService.GetSheetGroupBySheetGroupId(sheetGroupId.Value);
            if (sheetGroup == null)
            {
                return BadRequest(new CommonResponse<dynamic> { Result = false, Message = "此sheetGroupId不存在" });
            }

            var newSettingRow = _mapper.Map<HandoverSheetRow>(createSettingRowRequest);
            newSettingRow.MainSheetId = mainSheetId.Value;
            newSettingRow.SheetGroupId = sheetGroupId.Value;
            newSettingRow.SheetGroupTitle = sheetGroup.GroupTitle;
            newSettingRow.CreatorName = memberAndPermissionSetting.Member.DisplayName;
            var result = _handoverService.CreateHandoverSheetRow(newSettingRow);
            var response = new CommonResponse<HandoverSheetGroup>()
            {
                Result = result,
                Message = "",
                Data = null
            };
            return Ok(response);
        }

        [HttpPost("Category/create")]
        [Authorize]
        public IActionResult CreateSettingCategory(CreateSheetSettingCategoryRequest createCategoryListRequest)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            if (memberAndPermissionSetting == null || memberAndPermissionSetting.Member == null || permissionSetting == null || !permissionSetting.IsCreateHandover)
            {
                return Unauthorized(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }
            if (createCategoryListRequest.CategoryArray.Count == 0)
            {
                return BadRequest(new CommonResponse<dynamic> { Result = false, Message = "categoryArray不可為空" });
            }


            var mainSheetId = createCategoryListRequest.MainSheetId;
            var sheetGroupId = createCategoryListRequest.SheetGroupId;
            var sheetMain = _handoverService.GetSheetMainByMainSheetId(mainSheetId);
            if (sheetMain == null)
            {
                return BadRequest(new CommonResponse<dynamic> { Result = false, Message = "此MainSheet不存在" });
            }
            var sheetGroup = _handoverService.GetSheetGroupBySheetGroupId(sheetGroupId);
            if (sheetGroup == null)
            {
                return BadRequest(new CommonResponse<dynamic> { Result = false, Message = "此SheetGroup不存在" });
            }

            
            var (result,errorMsg) = _handoverService.CreateHandoverSheetCategorySetting(createCategoryListRequest);
            var response = new CommonResponse<HandoverSheetGroup>()
            {
                Result = result,
                Message = errorMsg,
                Data = null
            };
            return Ok(response);
        }

        [HttpPost("Category/update")]
        [Authorize]
        public IActionResult UpdateSettingCategory(UpdateSheetSettingCategoryRequest updateCategoryListRequest)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            if (memberAndPermissionSetting == null || memberAndPermissionSetting.Member == null || permissionSetting == null || !permissionSetting.IsCreateHandover)
            {
                return Unauthorized(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }
            //if (updateCategoryListRequest.CategoryArray.Count == 0)
            //{
            //    return BadRequest(new CommonResponse<dynamic> { Result = false, Message = "categoryArray不可為空" });
            //}


            var mainSheetId = updateCategoryListRequest.MainSheetId;
            var sheetGroupId = updateCategoryListRequest.SheetGroupId;
            var sheetMain = _handoverService.GetSheetMainByMainSheetId(mainSheetId);
            if (sheetMain == null)
            {
                return BadRequest(new CommonResponse<dynamic> { Result = false, Message = "此MainSheet不存在" });
            }
            var sheetGroup = _handoverService.GetSheetGroupBySheetGroupId(sheetGroupId);
            if (sheetGroup == null)
            {
                return BadRequest(new CommonResponse<dynamic> { Result = false, Message = "此SheetGroup不存在" });
            }


            var (result, errorMsg) = _handoverService.UpdateHandoverSheetCategorySetting(updateCategoryListRequest);
            var response = new CommonResponse<HandoverSheetGroup>()
            {
                Result = result,
                Message = errorMsg,
                Data = null
            };
            return Ok(response);
        }

        [HttpPost("Category/delete")]
        [Authorize]
        public IActionResult DeleteSettingCategory(DeleteSheetSettingCategoryRequest deleteCategoryListRequest)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            if (memberAndPermissionSetting == null || memberAndPermissionSetting.Member == null || permissionSetting == null || !permissionSetting.IsCreateHandover)
            {
                return Unauthorized(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }
            


            var mainSheetId = deleteCategoryListRequest.MainSheetId;
            var sheetGroupId = deleteCategoryListRequest.SheetGroupId;
            var sheetMain = _handoverService.GetSheetMainByMainSheetId(mainSheetId);
            if (sheetMain == null)
            {
                return BadRequest(new CommonResponse<dynamic> { Result = false, Message = "此MainSheet不存在" });
            }
            var sheetGroup = _handoverService.GetSheetGroupBySheetGroupId(sheetGroupId);
            if (sheetGroup == null)
            {
                return BadRequest(new CommonResponse<dynamic> { Result = false, Message = "此SheetGroup不存在" });
            }


            var (result, errorMsg) = _handoverService.DeleteHandoverSheetCategorySetting(deleteCategoryListRequest);
            var response = new CommonResponse<HandoverSheetGroup>()
            {
                Result = result,
                Message = errorMsg,
                Data = null
            };
            return Ok(response);
        }

        [HttpPost("Row/update")]
        [Authorize]
        public IActionResult UpdateSettingRow(CreateOrUpdateSheetSettingRowRequest updateSettingRowRequest)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            if (permissionSetting == null || !permissionSetting.IsUpdateHandover)
            {
                return Unauthorized(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }

            var validationResult = _updateSheetSettingRowRequestValidator.Validate(updateSettingRowRequest);

            if (!validationResult.IsValid)
            {
                return BadRequest(CommonResponse<dynamic>.BuildValidationFailedResponse(validationResult));
            }

            var updateSettingRow = _mapper.Map<HandoverSheetRow>(updateSettingRowRequest);
            var data = _handoverService.UpdateHandoverSheetRows(new List<HandoverSheetRow>() { updateSettingRow });
            var response = new CommonResponse<HandoverSheetRow>()
            {
                Result = true,
                Message = "",
                Data = data[0]
            };
            return Ok(response);
        }

        [HttpDelete("Row/delete/{sheetRowId}")]
        [Authorize]
        public IActionResult DeleteRow(int sheetRowId)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            if (memberAndPermissionSetting == null || permissionSetting == null || !permissionSetting.IsDeleteHandover)
            {
                return Unauthorized(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }
            _handoverService.InActiveHandoverSheetRow(sheetRowId);

            var response = new CommonResponse<dynamic>()
            {
                Result = true,
                Message = "",
                Data = null
            };
            return Ok(response);
        }


        [HttpPost("CategoryRank/update")]
        [Authorize]
        public IActionResult UpdateCategoryRank(UpdateCategoryRankRequest updateCategoryRankRequest)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            if (memberAndPermissionSetting == null || memberAndPermissionSetting.Member == null || permissionSetting == null || !permissionSetting.IsCreateHandover)
            {
                return Unauthorized(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }
            var categorySettings = _handoverService.GetCategorySettingsByCategoryIdList(updateCategoryRankRequest.CategoryIdSeq);

            var notExistingCategoryIdList = updateCategoryRankRequest.CategoryIdSeq.Except(categorySettings.Select(c => c.CategoryId)).ToList();
            if (notExistingCategoryIdList.Any())
            {
                return BadRequest(new CommonResponse<dynamic>
                {
                    Result = false,
                    Message = $"以下分類 ID 不存在: {string.Join(", ", notExistingCategoryIdList)}"
                });
            }
            if (categorySettings.Select(c => c.MainSheetId).Distinct().Count() > 1)
            {
                return BadRequest(new CommonResponse<dynamic>
                {
                    Result = false,
                    Message = "不可跨MainSheet"
                });
            }
            if (categorySettings.Select(c => c.SheetGroupId).Distinct().Count() > 1)
            {
                return BadRequest(new CommonResponse<dynamic>
                {
                    Result = false,
                    Message = "不可跨組別"
                });
            }



            var (result, errorMsg) = _handoverService.UpdateCategoryRank(updateCategoryRankRequest.CategoryIdSeq);
            var response = new CommonResponse<HandoverSheetGroup>()
            {
                Result = result,
                Message = errorMsg,
                Data = null
            };
            return Ok(response);
        }
    }
}
