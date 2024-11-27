using handover_api.Controllers.Dto;
using handover_api.Models;
using System.Text.Json.Serialization;

namespace handover_api.Service.ValueObject
{
    public class SheetSettingAndDetail
    {

        public List<GroupWithCategoryArrayDto>? HandoverSheetGroupList { get; set; } = new List<GroupWithCategoryArrayDto>();

        public string HandoverDetailId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int MainSheetId { get; set; }

        public string CreatorId { get; set; }

        public string CreatorName { get; set; }

        public DateTime? CreatedTime { get; set; }

        public DateTime? UpdatedTime { get; set; }

        public bool? IsActive { get; set; }

        public string Image { get; set; }

        public List<FileDetailInfo> Files { get; set; } = new List<FileDetailInfo>();
        public List<HandoverDetailReaderDto> HandoverDetailReader { get; set; } = new();
        public List<HandoverDetailHandler> HandoverDetailHandlers { get; set; } = new();
    }

    public class GroupWithCategoryArrayDto
    {
        public int SheetGroupId { get; set; }

        public string GroupTitle { get; set; }

        public int GroupRank { get; set; }

        public List<CategoryComponent> CategoryArray { get; set; } = new List<CategoryComponent>();
    }
}
