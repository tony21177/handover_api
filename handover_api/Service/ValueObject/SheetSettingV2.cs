using handover_api.Models;

namespace handover_api.Service.ValueObject
{
    public class SheetSettingV2
    {
        public int SheetId { get; set; }

        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public DateTime UpdatedTime { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public bool? IsActive { get; set; }

        public string Version { get; set; }

        public string SerialCode { get; set; }

        public string CreatorName { get; set; }

        public DateTime? CreatedTime { get; set; }

        public List<HandoverSheetGroupDtoV2>? HandoverSheetGroupList { get; set; } = new List<HandoverSheetGroupDtoV2>();
    }

    public class HandoverSheetGroupDtoV2
    {
        public string Id { get; set; }

        public int MainSheetId { get; set; }

        public int SheetGroupId { get; set; }

        public string GroupTitle { get; set; }

        public string GroupDescription { get; set; }

        public int GroupRank { get; set; }

        public bool? IsActive { get; set; }

        public string CreatorName { get; set; }

        public DateTime? CreatedTime { get; set; }

        public DateTime? UpdatedTime { get; set; }

        public List<CategorySettingDto> CategoryArray { get; set; } = new List<CategorySettingDto>();
    }
}
