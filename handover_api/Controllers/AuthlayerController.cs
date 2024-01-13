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
        public AuthlayerController(AuthLayerService authLayerService, IMapper mapper)
        {
            _authLayerService = authLayerService;
            _mapper = mapper;
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
            // TODO
            var data = _authLayerService.UpdateAuthlayers(authlayerList);

            var response = new CommonResponse<List<Authlayer>>()
            {
                Result = true,
                Message = "",
                Data = data
            };
            return response;
        }

    }
}

