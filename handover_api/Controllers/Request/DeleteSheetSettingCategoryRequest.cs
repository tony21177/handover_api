using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using handover_api.Service.ValueObject;

namespace handover_api.Controllers.Request
{
    public class DeleteSheetSettingCategoryRequest 
    {
        public int MainSheetId { get; set; }
        public int SheetGroupId { get; set; }
    }


}
