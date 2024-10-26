using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using handover_api.Service.ValueObject;

namespace handover_api.Controllers.Request
{
    public class CreateSheetSettingCategoryRequest
    {

        public int MainSheetId { get; set; }
        public int SheetGroupId { get; set; }
        public bool? IsActive { get; set; } = true;

        public List<Category> CategoryArray { get; set; } = new();
    }

    public class Category
    {
        public string? WeekDays { get; set; }
        public string? CategoryName { get; set; }
        public List<CategoryItem> ItemArray { get; set; } = new();
    }

    public class CategoryItem
    {
        public string ItemType { get; set; } = null!;
        public string? ItemTitle { get; set; }
        public string? ItemWidth { get; set; }
        public List<ItemOption> ItemOption { get; set; } = new();
    }
}
