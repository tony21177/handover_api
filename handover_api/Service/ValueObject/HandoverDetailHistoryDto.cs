using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace handover_api.Service.ValueObject
{
    public class HandoverDetailHistoryDto
    {
        
        public int Id { get; set; }

        public string HandoverDetailId { get; set; } = null!;

        public string? OldTitle { get; set; }

        public string? NewTitle { get; set; }

        public string? OldContent { get; set; }

        public string? NewContent { get; set; }

        public int MainSheetId { get; set; }

        [JsonIgnore]
        public string? OldJsonContent { get; set; }
        [JsonIgnore]
        public string? NewJsonContent { get; set; }

        public List<CategoryComponent>? OldCategoryArray { get; set; } = new List<CategoryComponent>();
        public List<CategoryComponent>? NewCategoryArray { get; set; } = new List<CategoryComponent>();

        public string? CreatorId { get; set; }

        public string? CreatorName { get; set; }

        public DateTime? CreatedTime { get; set; }

        public string? OldReaderUserIds { get; set; }

        public string? NewReaderUserIds { get; set; }

        public string? OldReaderUserNames { get; set; }

        public string? NewReaderUserNames { get; set; }

        public string? OldFileAttIds { get; set; }

        public string? NewFileAttIds { get; set; }

        public string Action { get; set; } = null!;
    }
}
