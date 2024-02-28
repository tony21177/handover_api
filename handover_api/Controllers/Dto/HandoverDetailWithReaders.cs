using handover_api.Models;

namespace handover_api.Controllers.Dto
{
    public class HandoverDetailWithReaders
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
        public List<HandoverDetailReader> HandoverDetailReader { get; set; } = new();
    }
}
