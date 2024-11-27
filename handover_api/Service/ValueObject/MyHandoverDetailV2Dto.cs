using handover_api.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace handover_api.Service.ValueObject
{
    public class MyHandoverDetailV2Dto
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

        [Column(TypeName = "timestamp")]
        public DateTime? CreatedTime { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime? UpdatedTime { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadTime { get; set; }
        public string FileAttIds { get; set; }
        public List<FileDetailInfo> Files { get; set; } = new List<FileDetailInfo>();
        public List<HandoverDetailHandler> HandoverDetailHandlers { get; set; } = new();
    }
}
