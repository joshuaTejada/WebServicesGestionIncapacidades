using Microsoft.AspNetCore.Mvc;
using WebServicesGestionIncapacidades.Core;
using WebServicesGestionIncapacidades.Models.Class.Response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebServicesGestionIncapacidades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        public IConfiguration _configuration;
        public CompanyController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // GET api/<CompanyController>/5
        [HttpGet("{TokenCompany}")]
        public CompanyDataResponse Get(string TokenCompany)
        {
            CompanyDataResponse responseModels = new();
            try
            {
                CompanyCore companyCore = new(_configuration);
                responseModels = companyCore.GetDataCompany(TokenCompany);
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
