using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using handover_api.Service.ValueObject;

namespace handover_api.Controllers.Request
{
    public class UpdateCategoryRankRequest
    {
        public List<string> CategoryIdSeq { get; set; } =null!;
    }

}
