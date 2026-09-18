using Microsoft.AspNetCore.Mvc;
using WebServicesGestionIncapacidades.Core;
using WebServicesGestionIncapacidades.Core.Security;
using WebServicesGestionIncapacidades.Models.Class.Request;
using WebServicesGestionIncapacidades.Models.Class.Response;

namespace WebServicesGestionIncapacidades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        public IConfiguration _configuration;
        public PersonController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // POST api/<PersonController>
        [HttpPost]
        public ResponseModels Post([FromBody] PersonRequest personRequest, [FromHeader] string token)
        {
            ResponseModels responseModels = new();
            try
            {
                PersonCore personCore = new(_configuration);
                responseModels = personCore.PostPerson(token, personRequest);
            }
            catch (Exception ex)
            {
                responseModels.CodeResponse = "500";
                responseModels.MessageResponse = ex.Message;
            }
            return responseModels;
        }

        [HttpGet]
        public string Get(string id)
        {
            SecurityCore securityCore = new(_configuration);            
            return securityCore.GenerateToken(id,"");
        }
    }
}