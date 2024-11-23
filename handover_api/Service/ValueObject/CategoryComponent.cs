using System.Text.Json;

namespace handover_api.Service.ValueObject
{

    public class CategoryComponent
    {
        public string CategoryId { get; set; } = null!;
        public string? WeekDays { get; set; }
        public int? MainSheetId { get; set; }
        public int? SheetGroupId { get; set; }
        public string? CategoryName { get; set; }
        public DateTime? CreatedTime { get; set; }    
        public DateTime? updatedTime { get; set; }    

        public List<CategoryItemValuesDto> ItemArray { get; set; } = new();

        public class CategoryItemValuesDto
        {
            public string ItemId { get; set; } = null!;
            public string ItemType { get; set; } = null!;
            public string? ItemTitle { get; set; }
            public string ItemWidth { get; set; } = null!;
            public List<ItemOptionAndValues> ItemOption { get; set; } = new();
            
        }

        public class ItemOptionAndValues
        {
            public string? OptionName { get; set; }
            public string? Type { get; set; } = null!;
            public string? Comment { get; set; }
            public int? Index { get; set; } = null!;
            public TableInfo? TableInfo { get; set; }
            public ItemOptionValues? Values { get; set; }

        }
        public class TableInfo
        {
            public int ColCount { get; set; }
            public int RowCount { get; set; }
            public List<string>? RowHeader { get; set; }
            public List<string>? ColumnHeader { get; set; }
        }

        public class ItemOptionValues
        {
            public List<string>? OptionValue { get; set; } = new List<string> ();
            public string? CommentValue { get; set; }
            public List<string>? TableValue { get; set; } = new List<string>();
            public List<string>? RemarkAssignUserNameValue { get; set; } = new List<string>();
            public List<string>? RemarkContentValue { get; set; } = new List<string>();
            public List<string>? RemarkAssignUserIDValue { get; set; } = new List<string>();
        }
    }
}
