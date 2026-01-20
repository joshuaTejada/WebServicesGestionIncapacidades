using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebServicesGestionIncapacidades.Core;

namespace WebServicesGestionIncapacidades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileSessionController : ControllerBase
    {
        public IConfiguration _configuration;
        public FileSessionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost("session")]
        public IActionResult CreateSession([FromHeader] string Token)
        {
            FileCore fileCore = new FileCore(_configuration);
            try
            {
                var sessionId = fileCore.CreateSession(Token);
                if (!sessionId.IsNullOrEmpty())
                    return Ok(new { sessionId });
                else
                    return NotFound();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
