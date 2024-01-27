using FluentValidation;
using handover_api.Controllers.Request;
using handover_api.Service;
using MaiBackend.PublicApi.Consts;

namespace handover_api.Controllers.Validator
{
    public class CreateOrUpdateMemberValidator : AbstractValidator<CreateOrUpdateMemberRequest>
    {
        private readonly AuthLayerService _authLayerService;

        public CreateOrUpdateMemberValidator(ActionTypeEnum action, AuthLayerService authLayerService)
        {
            if (action == ActionTypeEnum.Create)
            {
                RuleFor(x => x.Account).NotEmpty().WithMessage("account為必須");
                RuleFor(x => x.Password).NotEmpty().WithMessage("password為必須");
                RuleFor(x => x.DisplayName).NotEmpty().WithMessage("displayName為必須");
                RuleFor(x => x.IsActive).NotEmpty().WithMessage("isActive為必須");
                RuleFor(x => x.AuthValue).NotEmpty().WithMessage("authValue為必須");
                RuleFor(x => x.Uid).NotEmpty().WithMessage("uid為必須");
            }
            if (action == ActionTypeEnum.Update)
            {
                RuleFor(x => x.UserID).NotEmpty().WithMessage("userId為必須");
            }
            _authLayerService = authLayerService;
            ClassLevelCascadeMode = CascadeMode.Stop;
            // 驗證相對應的Workspace,Space,Folder,List是否存在
            RuleFor(x => x.AuthValue).Must(ExistAuthValue).WithMessage("此階層不存在");
        }

        private bool ExistAuthValue(short? authValue)
        {
            if (authValue == null) return false;
            var existAuthLayer = _authLayerService.GetByAuthValue(authValue.Value);
            return existAuthLayer != null;
        }

        private static bool BeValidValue(string? userId)
        {
            return userId != null;
        }
    }
}
