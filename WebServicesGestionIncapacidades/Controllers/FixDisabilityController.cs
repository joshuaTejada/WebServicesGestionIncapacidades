using Microsoft.AspNetCore.Mvc;
using WebServicesGestionIncapacidades.Core;
using WebServicesGestionIncapacidades.Models.Class.Response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebServicesGestionIncapacidades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FixDisabilityController : ControllerBase
    {
        public IConfiguration _configuration;
        public FixDisabilityController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        //GET api/<FixDisabilityController>/5
        [HttpGet]
        public FixDesabilityResponse Get([FromHeader] string Token, int id)
        {
            FixDesabilityResponse responseModels = new();
            try
            {
                DesabilitiesCore desabilitiesCore = new(_configuration);
                responseModels = desabilitiesCore.GetFixDesability(Token, id);
            }
            catch (Exception ex)
            {
                responseModels.CodeResponse = "500";
                responseModels.MessageResponse = ex.Message;
            }
            return responseModels;
        }
    }
}
