using FluentValidation;
using handover_api.Controllers.Request;
using handover_api.Service;

namespace handover_api.Controllers.Validator
{
    public class UpdateHandoverDetailRequestValidatorV2 : AbstractValidator<UpdateHandoverDetailRequestV2>
    {
        private readonly MemberService _memberService;

        public UpdateHandoverDetailRequestValidatorV2(MemberService memberService)
        {

            _memberService = memberService;

            RuleFor(x => x.HandoverDetailId).NotEmpty().WithMessage("handoverDetailId為必須");

            RuleFor(x => x.ReaderUserIds)
                .Must((request, userIds, context) => BeValidUserList(userIds, context))
                .WithMessage("以下 userId 為無效的 user: {InvalidUserIds}");

            ClassLevelCascadeMode = CascadeMode.Stop;
        }

        private bool BeValidUserList(List<string> userIds, ValidationContext<UpdateHandoverDetailRequestV2> context)
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
