using Microsoft.AspNetCore.Mvc;
using WebServicesGestionIncapacidades.Core.Security;
using WebServicesGestionIncapacidades.Models.Class.Response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebServicesGestionIncapacidades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class pruebasController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public pruebasController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
            // GET api/<pruebasController>/5
            [HttpGet("{nit}")]
        public string Get(string nit)
        {
            SecurityCore securityCore = new(_configuration);

            string Token = securityCore.GenerateToken(nit, "");

            return Token;
        }


    }
}