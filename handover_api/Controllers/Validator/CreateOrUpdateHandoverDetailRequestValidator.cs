using FluentValidation;
using handover_api.Controllers.Request;
using handover_api.Service;
using MaiBackend.PublicApi.Consts;

namespace handover_api.Controllers.Validator
{
    public class CreateOrUpdateHandoverDetailRequestValidator : AbstractValidator<CreateOrUpdateHandoverDetailRequest>
    {
        private readonly MemberService _memberService;

        public CreateOrUpdateHandoverDetailRequestValidator(ActionTypeEnum action, MemberService memberService)
        {
            
            _memberService = memberService;
            if (action == ActionTypeEnum.Create)
            {
                RuleFor(x => x.rowDetails).NotEmpty().WithMessage("rowDetails為必須");

            }
            if (action == ActionTypeEnum.Update)
            {

            }

            RuleFor(x => x.readerUserIds)
                .Must((request, userIds, context) => BeValidUserList(userIds, context))
                .WithMessage("以下 userId 為無效的 user: {InvalidUserIds}");

            ClassLevelCascadeMode = CascadeMode.Stop;
        }

        private bool BeValidUserList(List<string> userIds, ValidationContext<CreateOrUpdateHandoverDetailRequest> context)
        {
            if (userIds == null || userIds.Count == 0)
            {
                return true; // 允許空的 userIds
            }

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
    }
}
