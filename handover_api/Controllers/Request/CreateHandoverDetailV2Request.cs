using handover_api.Models;
using handover_api.Service.ValueObject;

namespace handover_api.Controllers.Request
{
    public class CreateHandoverDetailV2Request
    {
        public string Title { get; set; } = null!;

        public string? Content { get; set; }
        public List<CategoryComponent> categoryArray { get; set; } = new List<CategoryComponent>();

        public List<string> FileAttIds { get; set; } = new List<String>();
    }

}
