using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace handover_api.Service.ValueObject
{
    public class AnnouncementUnReadDto
    {

        public string UserId { get; set; } = null!;
        public string? UserName { get; set; }

        public string? PhotoUrl { get; set; }

        public int NotReadAnnouncementCount { get; set; }

        public List<NotReadAnnouncement> NotReadAnnouncementList { get; set; }
    }

    public class NotReadAnnouncement {

        public string? AnnounceId { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public DateTime? BeginPublishTime { get; set; }

        public DateTime? EndPublishTime { get; set; }

        public DateTime? BeginViewTime { get; set; }

        public DateTime? EndViewTime { get; set; }

        public string? CreatorId { get; set; }

        public string? CreatorName { get; set; }

        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }
}
