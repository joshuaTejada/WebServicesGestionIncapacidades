using Microsoft.AspNetCore.Mvc;
using WebServicesGestionIncapacidades.Core;
using WebServicesGestionIncapacidades.Models.Class.Request;
using WebServicesGestionIncapacidades.Models.Class.Response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebServicesGestionIncapacidades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisabilityController : ControllerBase
    {
        public IConfiguration _configuration;
        public DisabilityController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // POST api/<DisabilityController>
        [HttpPost]
        public DisabilityResponse Post([FromForm] DisabilitiesRequest disabilitiesRequest, [FromHeader] string token)
        {
            DisabilityResponse responseModels = new();
            try
            {
                DesabilitiesCore desabilitiesCore = new(_configuration);
                responseModels = desabilitiesCore.PostDisabilities(token, disabilitiesRequest);
            }
            catch (Exception ex)
            {
                responseModels.CodeResponse = "500";
                responseModels.MessageResponse = ex.Message;
            }
            return responseModels;
        }

        // POST api/<DisabilityController>
        [HttpPut]
        public DisabilityResponse Put([FromForm] DocumentRequest documentRequest, [FromHeader] string token)
        {
            DisabilityResponse responseModels = new();
            try
            {
                DesabilitiesCore desabilitiesCore = new(_configuration);
                responseModels = desabilitiesCore.PutDisabilities(token, documentRequest);
            }
            catch (Exception ex)
            {
                responseModels.CodeResponse = "500";
                responseModels.MessageResponse = ex.Message;
            }
            return responseModels;
        }

        [HttpGet]
        public DisabilityListResponse Get([FromHeader] string token, int ID)
        {
            DisabilityListResponse responseModels = new();
            try
            {
                DesabilitiesCore desabilitiesCore = new(_configuration);
                responseModels = desabilitiesCore.GetDisabilities(token, ID);
            }
            catch (Exception ex)
            {
                responseModels.CodeResponse = "500";
                responseModels.MessageResponse = ex.Message;
            }
            return responseModels;
        }


        //[HttpGet]
        //public async Task<bool> Get() {

        //    // Notificar al usuario de su incapacidad
        //    UtilitiesCore utilitiesCore = new(_configuration);
        //    var a = await utilitiesCore.SendEmail("informatica3@gigha.com.co", "Incapacidad recibida", "Pruebas", true);
        //    return a;
        //}
    }
}
