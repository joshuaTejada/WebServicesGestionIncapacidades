using Microsoft.AspNetCore.Mvc;
using WebServicesGestionIncapacidades.Core;
using WebServicesGestionIncapacidades.Models.Class.Response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebServicesGestionIncapacidades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachedRequiredController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AttachedRequiredController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // GET api/<AttachedRequiredController>/5
        [HttpGet]
        public AttachedResponse GetAttachedRequired([FromHeader] string Token, int IdTypeDisabilities, string IdEPS, int Transcribed, int Transit, string DiagnosticoCode)
        {
            AttachedResponse responseModels = new();
            try
            {
                DesabilitiesCore desabilitiesCore = new(_configuration);
                responseModels = desabilitiesCore.GetAttachedRequired(Token, IdTypeDisabilities, IdEPS, Transcribed, Transit, DiagnosticoCode);
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
