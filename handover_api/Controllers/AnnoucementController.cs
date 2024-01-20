using AutoMapper;
using FluentValidation;
using handover_api.Auth;
using handover_api.Common;
using handover_api.Controllers.Request;
using handover_api.Controllers.Validator;
using handover_api.Models;
using handover_api.Service;
using handover_api.Utils;
using MaiBackend.PublicApi.Consts;
using Microsoft.AspNetCore.Mvc;

namespace handover_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnnoucementController : Controller
    {
        private readonly AuthHelpers _authHelpers;
        private readonly MemberService _memberService;
        private readonly AnnouncementService _announcementService;
        private readonly IMapper _mapper;
        private readonly ILogger<AnnoucementController> _logger;
        private readonly IValidator<CreateOrUpdateAnnoucementRequest> _createAnnoucementRequest;
        private readonly IValidator<CreateOrUpdateAnnoucementRequest> _updateAnnoucementRequest;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly FileUploadService _fileUploadService;

        public AnnoucementController(AuthHelpers authHelpers, MemberService memberService, AnnouncementService announcementService, IMapper mapper, ILogger<AnnoucementController> logger, IWebHostEnvironment webHostEnvironment, FileUploadService fileUploadService)
        {
            _authHelpers = authHelpers;
            _memberService = memberService;
            _announcementService = announcementService;
            _mapper = mapper;
            _logger = logger;
            _createAnnoucementRequest = new CreateOrUpdateAnnouncementValidator(ActionTypeEnum.Create, memberService);
            _updateAnnoucementRequest = new CreateOrUpdateAnnouncementValidator(ActionTypeEnum.Create, memberService);
            _webHostEnvironment = webHostEnvironment;
            _fileUploadService = fileUploadService;
        }

        [HttpPost("create")]
        [AuthorizeRoles("1", "3", "5")]
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
            var validationResult = _createAnnoucementRequest.Validate(createAnnoucementRequest);
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
