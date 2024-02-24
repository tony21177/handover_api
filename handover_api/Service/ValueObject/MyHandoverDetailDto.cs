using System.ComponentModel.DataAnnotations.Schema;

namespace handover_api.Service.ValueObject
{
    public class MyHandoverDetailDto
    {
        public string HandoverDetailId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int MainSheetId { get; set; }

        public string JsonContent { get; set; }

        public string CreatorId { get; set; }

        public string CreatorName { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime? CreatedTime { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime? UpdatedTime { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadTime { get; set; }
    }
}
