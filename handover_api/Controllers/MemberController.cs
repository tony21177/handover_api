﻿using AutoMapper;
using FluentValidation;
using handover_api.Auth;
using handover_api.Common;
using handover_api.Controllers.Dto;
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
    public class MemberController : ControllerBase
    {
        private readonly MemberService _memberService;
        private readonly AuthLayerService _authLayerService;
        private readonly IMapper _mapper;
        private readonly ILogger<MemberController> _logger;
        private readonly IValidator<CreateOrUpdateMemberRequest> _createMemberRequestValidator;
        private readonly IValidator<CreateOrUpdateMemberRequest> _updateMemberRequestValidator;
        private readonly AuthHelpers _authHelpers;
        public MemberController(MemberService memberService, AuthLayerService authLayerService, IMapper mapper, ILogger<MemberController> logger,AuthHelpers authHelpers)
        {
            _memberService = memberService;
            _authLayerService = authLayerService;
            _mapper = mapper;
            _logger = logger;
            _createMemberRequestValidator = new CreateOrUpdateMemberValidator(ActionTypeEnum.Create, authLayerService);
            _updateMemberRequestValidator = new CreateOrUpdateMemberValidator(ActionTypeEnum.Update, authLayerService);
            _authHelpers = authHelpers;
        }

        [HttpGet("list")]
        [AuthorizeRoles("1", "3", "5")]
        public CommonResponse<List<MemberDto>> List()
        {
            var data = _memberService.GetAllMembers();
            var memberDtos = _mapper.Map<List<MemberDto>>(data);
            var response = new CommonResponse<List<MemberDto>>()
            {
                Result = true,
                Message = "",
                Data = memberDtos
            };
            return response;
        }

        [HttpGet("recipients")]
        [Authorize]
        public IActionResult ListRecipients()
        {
            var memberAndPermissionSetting = _authHelpers.GetMemberAndPermissionSetting(User);
            var permissionSetting = memberAndPermissionSetting?.PermissionSetting;

            if (memberAndPermissionSetting == null || permissionSetting == null || !permissionSetting.IsCreateAnnouce)
            {
                return Unauthorized(CommonResponse<dynamic>.BuildNotAuthorizeResponse());
            }

            var data = _memberService.GetAlRecipients();
           
            var response = new CommonResponse<List<Recipient>>()
            {
                Result = true,
                Message = "",
                Data = data
            };
            return Ok(response);
        }

        [HttpPost("create")]
        [AuthorizeRoles("1", "3", "5")]
        public async Task<IActionResult> Create(CreateOrUpdateMemberRequest createMemberRequset)
        {
            var validationResult = await _createMemberRequestValidator.ValidateAsync(createMemberRequset);

            var response = new CommonResponse<MemberDto>
            {
                Result = true,
                Message = "",
                Data = null
            };
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                response.Result = false;
                response.Message = string.Join(", ", errors);
                return BadRequest(response);
            }
            var newMember = _mapper.Map<Member>(createMemberRequset);
            newMember.UserId = Guid.NewGuid().ToString();

            newMember = _memberService.CreateMember(newMember!);
            var newMemberDto = _mapper.Map<MemberDto>(newMember);
            response.Data = newMemberDto;

            return Ok(response);
        }

        [HttpPost("update")]
        [AuthorizeRoles("1", "3", "5")]
        public async Task<IActionResult> Update(CreateOrUpdateMemberRequest updateMemberRequset)
        {
            var validationResult = await _updateMemberRequestValidator.ValidateAsync(updateMemberRequset);
            var response = new CommonResponse<MemberDto>
            {
                Result = true,
                Message = "",
                Data = null
            };
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                response.Result = false;
                response.Message = string.Join(", ", errors);
                return BadRequest(response);
            }

            var updateMember = _mapper.Map<Member>(updateMemberRequset);
            var updatedMember = _memberService.UpdateMember(updateMember);
            var updateedMemberDto = _mapper.Map<MemberDto>(updatedMember);
            response.Data = updateedMemberDto;

            return Ok(response);
        }

        [HttpDelete("delete/{userId}")]
        [AuthorizeRoles("1", "3", "5")]
        public IActionResult Delete(string userId)
        {

            _memberService.DeleteMember(userId);

            var response = new CommonResponse<Member>()
            {
                Result = true,
                Message = "",
                Data = null
            };
            return Ok(response);
        }
    }
}