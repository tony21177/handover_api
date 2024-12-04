using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace handover_api.Service.ValueObject
{
    public class HandoverUnReadDto
    {

        public string UserId { get; set; } = null!;
        public string? UserName { get; set; }

        public string? PhotoUrl { get; set; }

        public int NotReadHandoverCount { get; set; }

        public List<NotReadHandover> NotReadHandoverList { get; set; }
    }

    public class NotReadHandover {

        public string? HandoverDetailId { get; set; }

        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? JsonContent { get; set; }
        public string? CreatorId { get; set; }
        public string? CreatorName { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }
}
