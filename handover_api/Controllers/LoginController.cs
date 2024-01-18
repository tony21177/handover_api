using handover_api.Common;
using handover_api.Controllers.Request;
using handover_api.Models;
using handover_api.Service;
using handover_api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace handover_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly JwtHelpers _jwtHelpers;
        private readonly MemberService _memberService;
        private readonly AuthLayerService _authLayerService;

        public LoginController(JwtHelpers jwtHelpers, MemberService memberService, AuthLayerService authLayerService)
        {
            _jwtHelpers = jwtHelpers;
            _memberService = memberService;
            _authLayerService = authLayerService;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(LoginRequest loginRequest)
        {
            var result = new CommonResponse<Dictionary<string, string>>
            {
                Result = false,
                Message = "登入失敗",
                Data = null
            };
            var authValueAndPermissionSetting = ValidateUser(loginRequest);
            if (authValueAndPermissionSetting == null) return BadRequest(result);

            var token = _jwtHelpers.GenerateToken(loginRequest.Account, authValueAndPermissionSetting.AuthValue,authValueAndPermissionSetting.PermissionSetting);
            result.Result = true;
            result.Message = "登入成功";
            result.Data = new Dictionary<string, string> { { "token", token } };
            return Ok(result);
            
           
        }

        AuthValueAndPermissionSetting? ValidateUser(LoginRequest loginRequest)
        {
            var member = _memberService.GetMemberByAccount(loginRequest.Account);
            if (member == null) return null;
            if(member.Password != loginRequest.Password) return null;

            var authLayer = _authLayerService.GetByAuthValue(member.AuthValue);
            if (authLayer == null) return null; 
            PermissionSetting permissionSetting = new PermissionSetting
            {
                IsCheckReport = authLayer.IsCheckReport??false,
                IsCreateAnnouce = authLayer.IsCreateAnnouce ?? false,
                IsDeleteAnnouce = authLayer.IsDeleteAnnouce ?? false,
                IsCreateHandover = authLayer.IsCreateHandover ?? false,
                IsDeleteHandover = authLayer.IsDeleteHandover ?? false,
                IsHideAnnouce = authLayer.IsHideAnnouce ?? false,
                IsMemberControl = authLayer.IsMemberControl ?? false,
                IsUpdateAnnouce = authLayer.IsUpdateAnnouce ?? false,
                IsUpdateHandover = authLayer.IsUpdateHandover ?? false,
            };


            return new AuthValueAndPermissionSetting(authLayer.AuthValue, permissionSetting);
           
        }
    }
}
