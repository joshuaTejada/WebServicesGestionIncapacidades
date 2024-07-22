using Microsoft.AspNetCore.Mvc;
using WebServicesGestionIncapacidades.Core;
using WebServicesGestionIncapacidades.Models.Class.Response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebServicesGestionIncapacidades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiagnosisController : ControllerBase
    {
        public IConfiguration _configuration;
        public DiagnosisController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // GET: api/<DiagnosisController>
        [HttpGet]
        public DiagnosisResponse Get([FromHeader] string token, string valueFind)
        {
            DiagnosisResponse responseModels = new();
            try
            {
                DesabilitiesCore desabilitiesCore = new(_configuration);
                responseModels = desabilitiesCore.GetDiagnosis(token, valueFind);
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
