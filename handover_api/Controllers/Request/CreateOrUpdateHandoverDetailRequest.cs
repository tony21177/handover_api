using handover_api.Models;

namespace handover_api.Controllers.Request
{
    public class CreateOrUpdateHandoverDetailRequest
    {
        public List<RowDetail> rowDetails { get; set; } = new List<RowDetail>();

        public List<string> readerUserIds { get; set; } = new List<String>();
    }

    public class RowDetail
    {
        public int SheetRowId { get; set; }
        public string Status { get; set; }
        public string? Comment { get; set; }

        public HandoverSheetRow? HandoverSheetRowSetting { get; set; }
    }
}
