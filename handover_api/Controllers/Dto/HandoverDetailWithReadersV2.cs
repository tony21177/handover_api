using handover_api.Models;
using handover_api.Service.ValueObject;
using System.Text.Json.Serialization;

namespace handover_api.Controllers.Dto
{
    public class HandoverDetailWithReadersV2
    {
        public string HandoverDetailId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int MainSheetId { get; set; }
        [JsonIgnore]
        public string JsonContent { get; set; }
        public List<CategoryComponent> CategoryArray { get; set; } = new List<CategoryComponent>();

        public string CreatorId { get; set; }

        public string CreatorName { get; set; }

        public DateTime? CreatedTime { get; set; }

        public DateTime? UpdatedTime { get; set; }

        public bool? IsActive { get; set; }
        public List<FileDetailInfo> Files { get; set; } = new List<FileDetailInfo>();
        public List<HandoverDetailReaderDto> HandoverDetailReader { get; set; } = new();
    }
}
