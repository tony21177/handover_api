using FluentValidation;
using handover_api.Controllers.Request;
using stock_api.Common.Utils;

namespace handover_api.Controllers.Validator
{
    public class GetUnReadRequestValidator : AbstractValidator<GetUnReadRequest>
    {
        public GetUnReadRequestValidator() {

            RuleFor(x => x.StartDate).Must((request, date, context) => BeValidDate(date, context))
                    .WithMessage("無效格式日期");

            RuleFor(x => x.EndDate).Must((request, date, context) => BeValidDate(date, context))
                    .WithMessage("無效格式日期");
        }

        private static bool BeValidDate(string? date, ValidationContext<GetUnReadRequest> context)
        {
            if (date == null)
            {
                return true;
            }

            if (DateTimeHelper.ParseDateString(date) != null) return true;
            return false;
        }
    }
}
