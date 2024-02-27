using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace handover_api.Controllers.Dto
{
    public class HandoverDetailWithReadDto
    {
        public string HandoverDetailId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int MainSheetId { get; set; }
        public string JsonContent { get; set; }
        public string CreatorId { get; set; }
        public string CreatorName { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public bool? IsActive { get; set; }
        public List<string> FileAttIds { get; set; } = new List<string>();
        public bool? isRead { get; set; }
    }
}
