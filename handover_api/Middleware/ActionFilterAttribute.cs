﻿using handover_api.Common;
using handover_api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class PermissionFilterAttribute : ActionFilterAttribute
{
    private readonly AuthHelpers _authHelpers;

    public PermissionFilterAttribute(AuthHelpers authHelpers)
    {
        _authHelpers = authHelpers;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // 权限检查逻辑
        var user = context.HttpContext.User;
        var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(user);

        if (memberAndPermissionSetting == null)
        {
            context.Result = new UnauthorizedObjectResult(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            return;
        }

        // 将权限信息传递给 Controller，通过 HttpContext.Items
        context.HttpContext.Items["MemberAndPermissionSetting"] = memberAndPermissionSetting;
    }
}
