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
        public int? Index { get; set; } = null!;
        public Dictionary<string, object>? TableInfo { get; set; }
        public ItemOptionValues Values  {get;set;} = new ItemOptionValues();
    }

    public class ItemOptionValues
    {
        public List<string> optionValue {get;set;} = new List<string>();
        public string? commentValue {get;set;}
        public List<string> tableValue {get;set;} = new List<string>();
        public List<string> remarkContentValue {get;set;} = new List<string>();
        public List<string> remarkAssignUserNameValue {get;set;} = new List<string>();
        public List<string> remarkAssignUserIDValue {get;set;} = new List<string>();
    }
}
