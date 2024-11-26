using FluentValidation;
using handover_api.Common.Constant;
using handover_api.Controllers.Request;
using handover_api.Service;
using MaiBackend.PublicApi.Consts;

namespace handover_api.Controllers.Validator
{
    public class AddDetailHandlersValidator : AbstractValidator<AddDetailHandlersRequest>
    {
        private readonly MemberService _memberService;

        public AddDetailHandlersValidator( MemberService memberService)
        {
            _memberService = memberService;

            RuleFor(x => x.UserList)
            // 第一规则：先检查 UserId 是否有效
            .Must((request, userList, context) => BeValidUserList(userList, context))
            .WithMessage("以下 userId 為無效的 user: {InvalidUserIds}")
            // 当 UserList 验证通过时，才继续验证 Type
            .DependentRules(() =>
            {
                RuleFor(x => x.UserList)
                    .Must((request, userList) => BeValidType(userList))
                    .WithMessage($"type必須為{string.Join(",", CommonConstants.DetailHandlerUserType.GetAllValues())}");
            });
        }

        private bool BeValidUserList(List<UserRequest>  userList, ValidationContext<AddDetailHandlersRequest> context)
        {
            var userIds = userList.Select(u => u.UserId).ToList();
            var activeMemberList = _memberService.GetActiveMembersByUserIds(userIds);
            var notExistUserIds = userIds.Except(activeMemberList.Select(m => m.UserId)).ToList();

            if (notExistUserIds.Any())
            {
                var errorMessage = $"{string.Join(",", notExistUserIds)}";
                context.MessageFormatter.AppendArgument("InvalidUserIds", errorMessage);
                return false;
            }
            return true;
        }


        private static bool BeValidType(List<UserRequest> userList)
        {
            foreach (var user in userList)
            {
                if (!CommonConstants.DetailHandlerUserType.GetAllValues().Contains(user.Type))
                {
                    return false;
                }
            }
            return true ;
        }
    }
}
