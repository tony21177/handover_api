using handover_api.Models;

namespace handover_api.Controllers.Dto
{
    public class AnnouncementWithAttachments
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? BeginPublishTime { get; set; }
        public DateTime? EndPublishTime { get; set; }
        public DateTime? BeginViewTime { get; set; }
        public DateTime? EndViewTime { get; set; }
        public bool IsActive { get; set; }
        public string AnnounceId { get; set; }
        public string CreatorId { get; set; }
        public string CreatorName { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public List<AnnounceAttachment> AnnounceAttachments { get; set; } = new List<AnnounceAttachment>();

        public bool? IsRead { get; set; }

        public string UserId { get; set; } = null!;

        public bool IsBookToTop { get; set; } = false;

        public bool IsRemind { get; set; } = false;
    }
}