﻿using AutoMapper;
using handover_api.Common;
using handover_api.Controllers.Dto;
using handover_api.Models;
using handover_api.Service;
using handover_api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace handover_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(PermissionFilterAttribute))]
    public class DashboardController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<DashboardController> _logger;
        private readonly AuthHelpers _authHelpers;
        private readonly HandoverService _handoverService;
        private readonly MemberService _memberService;
        private readonly AnnouncementService _announcementService;

        public DashboardController(IMapper mapper, ILogger<DashboardController> logger, AuthHelpers authHelpers, HandoverService handoverService, MemberService memberService, AnnouncementService announcementService)
        {
            _mapper = mapper;
            _logger = logger;
            _authHelpers = authHelpers;
            _handoverService = handoverService;
            _memberService = memberService;
            _announcementService = announcementService;
        }

        [HttpGet("unread/my")]
        [Authorize]
        public IActionResult GetMyHandoverDetail()
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            Member reader = memberAndPermissionSetting.Member;
            var handoverDetailDtoList = _handoverService.GetMyHandoverDetailDtoList(reader.UserId);
            List<HandoverDetail> unreadHandoverDetails = _handoverService.GetUnreadHandoverDetails(reader);
            List<Announcement> unreadAnnoucements = _announcementService.GetUnreadAnnouncement(reader);

            UnreadDto unreadDto = new UnreadDto
            {
                UnreadAnnouncementCount = unreadAnnoucements.Count,
                UnreadHandoverCount = unreadHandoverDetails.Count,
                UnreadAnnouncements = unreadAnnoucements,
                UnreadHandoverDetails = unreadHandoverDetails,
            };

            return Ok(new CommonResponse<UnreadDto>
            {
                Result = true,
                Data = unreadDto
            });
        }
    }
}
