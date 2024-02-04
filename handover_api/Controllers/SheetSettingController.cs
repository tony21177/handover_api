using AutoMapper;
using handover_api.Common;
using handover_api.Service;
using handover_api.Service.ValueObject;
using handover_api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace handover_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SheetSettingController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<SheetSettingController> _logger;
        private readonly AuthHelpers _authHelpers;
        private readonly HandoverService _handoverService;

        public SheetSettingController(IMapper mapper, ILogger<SheetSettingController> logger, AuthHelpers authHelpers, HandoverService handoverService)
        {
            _mapper = mapper;
            _logger = logger;
            _authHelpers = authHelpers;
            _handoverService = handoverService;
        }

        [HttpGet("list")]
        [Authorize]
        public IActionResult List()
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;


            var data = _handoverService.GetAllSettings();
            var response = new CommonResponse<List<SheetSettingDto>>()
            {
                Result = true,
                Message = "",
                Data = data
            };
            return Ok(response);
        }
    }
}
