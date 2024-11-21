using handover_api.Models;
using handover_api.Service.ValueObject;
using System.Text.Json.Serialization;

namespace handover_api.Controllers.Dto
{
    public class HandoverDetailWithReadV2Dto
    {
        public string HandoverDetailId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int MainSheetId { get; set; }
        public int SheetGroupId { get; set; }
        [JsonIgnore]
        public string? JsonContent { get; set; }
        public List<CategoryComponent> CategoryArray { get; set; } = new List<CategoryComponent>();
        public string CreatorId { get; set; }
        public string CreatorName { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public bool? IsActive { get; set; }
        [JsonIgnore]
        public string FileAttIds { get; set; }
        public List<FileDetailInfo> Files { get; set; } = new List<FileDetailInfo>();
        public bool? IsRead { get; set; }
    }
}
