using System.Text.Json;

namespace handover_api.Service.ValueObject
{
    public class CategoryItemDto
    {
        public string ItemId { get; set; } = null!;
        public string ItemType { get; set; } = null!;
        public string? ItemTitle { get; set; } 
        public string ItemWidth { get; set; } = null!;
        public List<ItemOption> ItemOption { get; set; } = new();
    }

    public class ItemOption
    {
        public string? OptionName { get; set; }
        public string? Type { get; set; } = null!;
        public string? Comment { get; set; }
        public Dictionary<string, object>? TableInfo { get; set; }
    }
}
