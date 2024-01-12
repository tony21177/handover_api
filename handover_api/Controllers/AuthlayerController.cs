using handover_api.Common;
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
        public AuthlayerController(AuthLayerService authLayerService)
        {
            _authLayerService = authLayerService;
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

    }
}

