using AutoMapper;
using FluentValidation;
using handover_api.Common;
using handover_api.Controllers.Request;
using handover_api.Controllers.Validator;
using handover_api.Models;
using handover_api.Service;
using handover_api.Service.ValueObject;
using handover_api.Utils;
using MaiBackend.PublicApi.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace handover_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SheetSettingController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<SheetSettingController> _logger;
        private readonly AuthHelpers _authHelpers;
        private readonly HandoverService _handoverService;
        private readonly IValidator<CreateOrUpdateSheetSettingMainRequest> _createSheetSettingMainRequestValidator;
        private readonly IValidator<CreateOrUpdateSheetSettingMainRequest> _updateSheetSettingMainRequestValidator;
        private readonly FileUploadService _fileUploadService;

        public SheetSettingController(IMapper mapper, ILogger<SheetSettingController> logger, AuthHelpers authHelpers, HandoverService handoverService, FileUploadService fileUploadService)
        {
            _mapper = mapper;
            _logger = logger;
            _authHelpers = authHelpers;
            _handoverService = handoverService;
            _createSheetSettingMainRequestValidator = new CreateOrUpdateSheetSettingMainRequestValidator(ActionTypeEnum.Create);
            _updateSheetSettingMainRequestValidator = new CreateOrUpdateSheetSettingMainRequestValidator(ActionTypeEnum.Update);
            _fileUploadService = fileUploadService;
        }

        [HttpGet("list")]
        [Authorize]
        public IActionResult List()
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;


            var data = _handoverService.GetAllSettings();
            var response = new CommonResponse<List<SheetSetting>>()
            {
                Result = true,
                Message = "",
                Data = data
            };
            return Ok(response);
        }


        [HttpPost("Images/upload")]
        [Authorize]
        public async Task<IActionResult> UploadImage([FromForm] UploadFilesRequest uploadFilesRequest)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            if (permissionSetting == null || (!permissionSetting.IsCreateHandover && !permissionSetting.IsUpdateHandover))
            {
                return Unauthorized(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }

            var fileDetails = await _fileUploadService.PostFilesAsync(uploadFilesRequest.Files, new List<string> { "handover" });
            var fileDetailInfos = _mapper.Map<List<FileDetailInfo>>(fileDetails);
            bool result = _fileUploadService.AddFileDetailInfo(fileDetailInfos);

            return Ok(new CommonResponse<List<FileDetailInfo>>
            {
                Result = result,
                Message = result ? "" : "上傳失敗",
                Data = fileDetailInfos
            });
        }
        [HttpGet("Images/{attid}")]
        public async Task<IActionResult> DownloadImage(string attid)
        {
            var fileDetail = _fileUploadService.GetFileDetail(attid);
            if (fileDetail == null)
            {
                return NotFound();
            }
            var fileStream = _fileUploadService.Download(fileDetail);
            return fileStream;
        }

        [HttpPost("create")]
        [Authorize]
        public IActionResult CreateSettingMain(CreateOrUpdateSheetSettingMainRequest createSettingMainRequest)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            if (memberAndPermissionSetting == null || memberAndPermissionSetting.Member == null || permissionSetting == null || !permissionSetting.IsCreateHandover)
            {
                return Unauthorized(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }
            var validationResult = _createSheetSettingMainRequestValidator.Validate(createSettingMainRequest);

            if (!validationResult.IsValid)
            {
                return BadRequest(CommonResponse<dynamic>.BuildValidationFailedResponse(validationResult));
            }


            var newSettingMain = _mapper.Map<HandoverSheetMain>(createSettingMainRequest);
            newSettingMain.CreatorName = memberAndPermissionSetting.Member.DisplayName;
            var result = _handoverService.CreateHandoverSheetMain(newSettingMain);
            var response = new CommonResponse<HandoverSheetMain>()
            {
                Result = result,
                Message = "",
                Data = null
            };
            return Ok(response);
        }

        [HttpPost("update")]
        [Authorize]
        public IActionResult UpdateSettingMain(List<CreateOrUpdateSheetSettingMainRequest> updateSettingMainRequestList)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            if (permissionSetting == null || !permissionSetting.IsUpdateHandover)
            {
                return Unauthorized(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }

            foreach (var updateSettingMainRequest in updateSettingMainRequestList)
            {
                var validationResult = _updateSheetSettingMainRequestValidator.Validate(updateSettingMainRequest);

                if (!validationResult.IsValid)
                {
                    return BadRequest(CommonResponse<dynamic>.BuildValidationFailedResponse(validationResult));
                }
            }

            var updateSettingMainList = _mapper.Map<List<HandoverSheetMain>>(updateSettingMainRequestList);
            var data = _handoverService.UpdateHandoverSheetMains(updateSettingMainList);
            var response = new CommonResponse<List<HandoverSheetMain>>()
            {
                Result = true,
                Message = "",
                Data = data
            };
            return Ok(response);
        }

        [HttpDelete("delete/{sheetId}")]
        [Authorize]
        public IActionResult Delete(int sheetId)
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;
            if (memberAndPermissionSetting == null || permissionSetting == null || !permissionSetting.IsDeleteHandover)
            {
                return Unauthorized(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }
            _handoverService.DeleteHandoverSheetMain(sheetId);

            var response = new CommonResponse<dynamic>()
            {
                Result = true,
                Message = "",
                Data = null
            };
            return Ok(response);
        }
    }
}
