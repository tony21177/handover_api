using AutoMapper;
using handover_api.Common;
using handover_api.Controllers.Request;
using handover_api.Models;
using handover_api.Service;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace handover_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthlayerController : Controller
    {
        private readonly AuthLayerService _authLayerService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthlayerController> _logger;
        public AuthlayerController(AuthLayerService authLayerService, IMapper mapper, ILogger<AuthlayerController> logger)
        {
            _authLayerService = authLayerService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("list")]
        public CommonResponse<List<Authlayer>> List()
        {
            var data = _authLayerService.GetAllAuthlayers();
            var response = new CommonResponse<List<Authlayer>>()
            {
                Result = true,
                Message = "",
                Data = data
            };
            return response;
        }

        [HttpPost("update")]
        public CommonResponse<List<Authlayer>> Update(List<UpdateAuthlayerRequest> updateAuthlayerListRequest)
        {
            var authlayerList = _mapper.Map<List<Authlayer>>(updateAuthlayerListRequest);
            var data = _authLayerService.UpdateAuthlayers(authlayerList);

            var response = new CommonResponse<List<Authlayer>>()
            {
                Result = true,
                Message = "",
                Data = data
            };
            return response;
        }

        [HttpPost("create")]
        public CommonResponse<Authlayer> Create(CreateAuthlayerRequest createAuthlayerRequest)
        {
            var newAuthLayer = _mapper.Map<Authlayer>(createAuthlayerRequest);
            // TODO
            var data = _authLayerService.AddAuthlayer(newAuthLayer);

            var response = new CommonResponse<Authlayer>()
            {
                Result = true,
                Message = "",
                Data = data
            };
            return response;
        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {

            _authLayerService.DeleteAuthLayer(id);

            var response = new CommonResponse<Authlayer>()
            {
                Result = true,
                Message = "",
                Data = null
            };
            return Ok(response);
        }

    }
}

