using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace handover_api.Service.ValueObject
{
    public class CategorySettingDto
    {
        public string CategoryId { get; set; } = null!;

        public int? MainSheetId { get; set; }

        public int? SheetGroupId { get; set; }

        public string? WeekDays { get; set; }

        public string CategoryName { get; set; } = null!;

        public DateTime? CreatedTime { get; set; }

        public DateTime? UpdatedTime { get; set; }

        public List<CategoryItemDto> ItemArray { get; set; } = new ();
    }
}
