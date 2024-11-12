using FluentValidation;
using handover_api.Controllers.Request;
using handover_api.Service;
using handover_api.Service.ValueObject;
using MaiBackend.PublicApi.Consts;

namespace handover_api.Controllers.Validator
{
    public class CreateHandoverDetailRequestV2Validator : AbstractValidator<CreateHandoverDetailV2Request>
    {
        private readonly MemberService _memberService;

        public CreateHandoverDetailRequestV2Validator(ActionTypeEnum action, MemberService memberService)
        {

            _memberService = memberService;
            if (action == ActionTypeEnum.Create)
            {
                RuleFor(x => x.Title).NotEmpty().WithMessage("title為必須");
            }


            ClassLevelCascadeMode = CascadeMode.Stop;
        }

        
    }
}
