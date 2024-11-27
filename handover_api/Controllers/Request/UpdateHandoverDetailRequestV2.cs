using handover_api.Service.ValueObject;

namespace handover_api.Controllers.Request
{
    public class UpdateHandoverDetailRequestV2
    {
        public string HandoverDetailId { get; set; }
        public string? Title { get; set; }

        public string? Content { get; set; }
        public List<CategoryComponent>? CategoryArray { get; set; }
        public List<string> ReaderUserIds { get; set; } = new List<String>();
        public List<string> FileAttIds { get; set; } = new List<String>();
    }


}
