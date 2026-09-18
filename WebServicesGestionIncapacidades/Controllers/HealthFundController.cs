using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebServicesGestionIncapacidades.Core;
using WebServicesGestionIncapacidades.Models.Class.Response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebServicesGestionIncapacidades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthFundController : ControllerBase
    {

        public IConfiguration _configuration;
        public HealthFundController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public healthFundResponse Get(int IdTypeDisability,string ID, [FromHeader]string Token)
        {
            healthFundResponse responseModels = new();
            try
            {
                DesabilitiesCore desabilitiesCore = new(_configuration);
                responseModels = desabilitiesCore.GetHealthFund(Token, IdTypeDisability, ID);
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
