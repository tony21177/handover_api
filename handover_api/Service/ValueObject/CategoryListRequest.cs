using System.Text.Json;

namespace handover_api.Service.ValueObject
{

    public class CategoryListRequest
    {
        public string CategoryId { get; set; } = null!;

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
            public Dictionary<string, object>? TableInfo { get; set; }

            public ItemOptionValues? Values { get; set; }
        }

        public class ItemOptionValues
        {
            public List<string>? OptionValue { get; set; }
            public string? CommentValue { get; set; }
            public List<string>? TableValue { get; set; }
            public List<string>? RemarkAssignUserNameValue { get; set; }

            public List<string>? RemarkAssignUserIDValue { get; set; }
        }
    }
}
