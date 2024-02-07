using handover_api.Models;

namespace handover_api.Controllers.Dto
{
    public class AnnouncementWithAttachmentsReaders : AnnouncementWithAttachments
    {
        

        public List<MemberDto> ReaderUserList { get; set; } = new List<MemberDto>();

    }
}
