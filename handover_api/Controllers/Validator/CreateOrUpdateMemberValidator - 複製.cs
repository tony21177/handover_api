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

            }
            if (action == ActionTypeEnum.Update)
            {
                RuleFor(x => x.UserID).Must(BeValidValue).WithMessage("userId為必須");
            }
            _authLayerService = authLayerService;
            ClassLevelCascadeMode = CascadeMode.Stop;
            // 驗證相對應的Workspace,Space,Folder,List是否存在
            RuleFor(x => x.AuthValue).Must(ExistAuthValue).WithMessage("此階層不存在");
        }

        private bool ExistAuthValue(short authValue)
        {
            var existAuthLayer = _authLayerService.GetByAuthValue(authValue);
            return existAuthLayer != null;
        }

        private static bool BeValidValue(string? userId)
        {
            return userId != null;
        }
    }
}
