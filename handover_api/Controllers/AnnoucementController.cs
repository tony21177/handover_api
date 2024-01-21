using AutoMapper;
using FluentValidation;
using handover_api.Auth;
using handover_api.Common;
using handover_api.Controllers.Dto;
using handover_api.Controllers.Request;
using handover_api.Controllers.Validator;
using handover_api.Models;
using handover_api.Service;
using handover_api.Utils;
using MaiBackend.PublicApi.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace handover_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AnnoucementController : Controller
    {
        private readonly AuthHelpers _authHelpers;
        private readonly MemberService _memberService;
        private readonly AnnouncementService _announcementService;
        private readonly IMapper _mapper;
        private readonly ILogger<AnnoucementController> _logger;
        private readonly IValidator<CreateOrUpdateAnnoucementRequest> _createAnnoucementRequestValidator;
        private readonly IValidator<CreateOrUpdateAnnoucementRequest> _updateAnnoucementRequestValidator;
        private readonly IValidator<ListAnnoucementRequest> _listAnnoucementRequestValidator;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly FileUploadService _fileUploadService;

        public AnnoucementController(AuthHelpers authHelpers, MemberService memberService, AnnouncementService announcementService, IMapper mapper, ILogger<AnnoucementController> logger, IWebHostEnvironment webHostEnvironment, FileUploadService fileUploadService)
        {
            _authHelpers = authHelpers;
            _memberService = memberService;
            _announcementService = announcementService;
            _mapper = mapper;
            _logger = logger;
            _createAnnoucementRequestValidator = new CreateOrUpdateAnnouncementValidator(ActionTypeEnum.Create, memberService);
            _updateAnnoucementRequestValidator = new CreateOrUpdateAnnouncementValidator(ActionTypeEnum.Create, memberService);
            _listAnnoucementRequestValidator = new ListAnnouncementRequestValidator();
            _webHostEnvironment = webHostEnvironment;
            _fileUploadService = fileUploadService;
        }

        [HttpPost("create")]
        [Authorize]
        public IActionResult Create(CreateOrUpdateAnnoucementRequest createAnnoucementRequest)
        {
            var response = new CommonResponse<Announcement>
            {
                Result = true,
                Message = "",
                Data = null
            };
            // 權限控管
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            if (memberAndPermissionSetting == null) return Unauthorized(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            var permisionSetting = memberAndPermissionSetting.PermissionSetting;
            if (permisionSetting.IsCreateAnnouce == false)
            {
                return Unauthorized(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }

            // 參數驗證
            var validationResult = _createAnnoucementRequestValidator.Validate(createAnnoucementRequest);
            if (!validationResult.IsValid)
            {
                // 從 FluentValidation 的 ValidationResult 中獲取錯誤信息
                var errorMessage = validationResult.Errors.FirstOrDefault()?.ErrorMessage;
                response.Result = false;
                response.Message = errorMessage ?? "參數錯誤";
                return BadRequest(response);
            }

            // 業務邏輯
            var newAnnouncement = _mapper.Map<Announcement>(createAnnoucementRequest);
            newAnnouncement = _announcementService.CreateAnnouncement(newAnnouncement, createAnnoucementRequest.ReaderUserIdList, memberAndPermissionSetting.Member, createAnnoucementRequest.AttIdList);
            if (newAnnouncement == null)
            {
                response.Result = false;
                response.Message = "創建公告失敗";
                return Ok(response);
            }
            response.Result = true;
            response.Data = newAnnouncement;

            return Ok(response);
        }

        [HttpPost("list")]
        [Authorize]
        public IActionResult ListAnnouncements(ListAnnoucementRequest listAnnoucementRequest)
        {
            //if (User.Identity == null || !User.Identity.IsAuthenticated)
            //{
            //    return Unauthorized(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            //}

            // 參數驗證
            var validationResult = _listAnnoucementRequestValidator.Validate(listAnnoucementRequest);
            if (!validationResult.IsValid)
            {
                // 從 FluentValidation 的 ValidationResult 中獲取錯誤信息
                var errorMessage = validationResult.Errors.FirstOrDefault()?.ErrorMessage;
                return BadRequest(new CommonResponse<dynamic>
                {
                    Result = false,
                    Message = errorMessage ?? "參數錯誤"
                });
            }


            var announcements = _announcementService.GetFilteredAnnouncements(listAnnoucementRequest);
            // Extract unique AnnounceIds from the announcements
            var uniqueAnnounceIds = announcements.Select(a => a.AnnounceId).Distinct().ToList();

            // Retrieve attachments based on the unique AnnounceIds
            var attachments = _announcementService.GetAttachmentsByAnnounceIds(uniqueAnnounceIds);
            // Map attachments to corresponding announcements
            var result = announcements.Select(announcement =>
            {
                var announceAttachments = attachments
                    .Where(a => a.AnnounceId == announcement.AnnounceId)
                    .ToList();

                return new AnnouncementWithAttachments
                {
                    Id = announcement.Id,
                    Title = announcement.Title,
                    Content = announcement.Content,
                    BeginPublishTime = announcement.BeginPublishTime,
                    EndPublishTime = announcement.EndPublishTime,
                    BeginViewTime = announcement.BeginViewTime,
                    EndViewTime = announcement.EndViewTime,
                    IsActive = announcement.IsActive ?? false,
                    AnnounceId = announcement.AnnounceId,
                    CreatorId = announcement.CreatorId,
                    CreatorName = announcement.CreatorName,
                    CreatedTime = announcement.CreatedTime,
                    UpdatedTime = announcement.UpdatedTime,
                    AnnounceAttachments = announceAttachments
                };
            }).ToList();

            return Ok(new CommonResponse<List<AnnouncementWithAttachments>>
            {
                Result = true,
                Data = result
            });
        }


        [HttpPost("Attachment/upload")]
        [AuthorizeRoles("1", "3", "5")]
        public async Task<IActionResult> UploadAttatchment([FromForm] UploadAnnouncementAttachmentRequest uploadAnnouncementAttachmentRequest)
        {
            var fileDetails = await _fileUploadService.PostFilesAsync(uploadAnnouncementAttachmentRequest.Files, new List<string> { "announcement" });
            List<AnnounceAttachment> announceAttachments = new List<AnnounceAttachment>();
            if (fileDetails.Count > 0)
            {
                announceAttachments = _announcementService.AnnounceAttachments(fileDetails);
            }

            return Ok(new CommonResponse<List<AnnounceAttachment>>
            {
                Result = true,
                Data = announceAttachments
            });
        }

    }
}
