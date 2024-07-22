using Microsoft.AspNetCore.Mvc;
using WebServicesGestionIncapacidades.Core;
using WebServicesGestionIncapacidades.Models.Class.Request;
using WebServicesGestionIncapacidades.Models.Class.Response;

namespace WebServicesGestionIncapacidades.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValidatePersonController : ControllerBase
    {
        public IConfiguration _configuration;
        public ValidatePersonController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost]
        public ResponseModels Post([FromBody] PersonRequest personRequest, [FromHeader] string Token)
        {
            ResponseModels responseModels = new();
            try
            {
                PersonCore personCore = new(_configuration);
                responseModels = personCore.ValidatePerson(Token, personRequest);
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
