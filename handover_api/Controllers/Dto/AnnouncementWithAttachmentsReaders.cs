using handover_api.Models;

namespace handover_api.Controllers.Dto
{
    public class AnnouncementWithAttachmentsReaders : AnnouncementWithAttachments
    {
        

        public List<AnnouceReaderMemberDto> ReaderUserList { get; set; } = new List<AnnouceReaderMemberDto>();

    }
}
