﻿using AutoMapper;
using handover_api.Auth;
using handover_api.Common;
using handover_api.Controllers.Request;
using handover_api.Controllers.Validator;
using handover_api.Models;
using handover_api.Service;
using handover_api.Utils;
using MaiBackend.Common.AutoMapper;
using MaiBackend.PublicApi.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace handover_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HandoverController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<HandoverController> _logger;
        private readonly AuthHelpers _authHelpers;
        private readonly HandoverService _handoverService;
        private readonly MemberService _memberService;
        private readonly CreateOrUpdateHandoverDetailRequestValidator _createHandoverDetailRequestValidator;

        public HandoverController(IMapper mapper, ILogger<HandoverController> logger, AuthHelpers authHelpers, HandoverService handoverService, MemberService memberService)
        {
            _mapper = mapper;
            _logger = logger;
            _authHelpers = authHelpers;
            _handoverService = handoverService;
            _memberService = memberService;
            _createHandoverDetailRequestValidator = new CreateOrUpdateHandoverDetailRequestValidator(ActionTypeEnum.Create, _memberService);
        }


        [HttpPost("create")]
        [AuthorizeRoles("1", "3", "5")]
        public IActionResult CreateHandover(CreateOrUpdateHandoverDetailRequest createHandoverDetailRequest)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            Member creatorMember = memberAndPermissionSetting.Member;

            // 參數驗證
            var validationResult = _createHandoverDetailRequestValidator.Validate(createHandoverDetailRequest);
            if (!validationResult.IsValid)
            {
                return BadRequest(CommonResponse<dynamic>.BuildValidationFailedResponse(validationResult));
            }

            // 驗證是否同屬一個main
            var sheetRowIdList = createHandoverDetailRequest.rowDetails.Select(rd => rd.SheetRowId).ToList();
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

            List<HandoverSheetRow> neededSheetRow = _handoverService.GetSheetRowsByMainSheetId(matchedSheetMainIdList[0]).Where(row => row.IsActive == true).ToList();
            var neededSheetRowCount = neededSheetRow.Count;
            if (neededSheetRowCount != createHandoverDetailRequest.rowDetails.Count)
            {
                return BadRequest(new CommonResponse<dynamic>
                {
                    Result = false,
                    Message = $"交班單的row筆數:{createHandoverDetailRequest.rowDetails.Count}不對,需要{neededSheetRowCount}筆"
                });
            }

            List<Member> readerMemberList = _memberService.GetMembersByUserIdList(createHandoverDetailRequest.readerUserIds);

            if (readerMemberList.Find(m => m.UserId == creatorMember.UserId) == null)
            {
                readerMemberList.Add(creatorMember);
            }

            var createdJsonContent = _handoverService.CreateHandOverDetail(matchedSheetMainIdList[0], createHandoverDetailRequest.rowDetails, createHandoverDetailRequest.Title, createHandoverDetailRequest.Content, readerMemberList, creatorMember);

            return Ok(new CommonResponse<string?>
            {
                Result = createdJsonContent != null,
                Data = createdJsonContent,
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult SearchHandoverDetails(SearchHandoverDetailRequest searchHandoverDetailRequest)
        {
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


            var data = _handoverService.SearchHandoverDetails(searchHandoverDetailRequest.MainSheetId, startDate, endDate,
                searchHandoverDetailRequest.PaginationCondition, searchHandoverDetailRequest.SearchString);

            return Ok(new CommonResponse<List<HandoverDetail>>
            {
                Result = true,
                Data = data
            });
        }
    }
}