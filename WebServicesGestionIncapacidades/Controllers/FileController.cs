using Microsoft.AspNetCore.Mvc;
using WebServicesGestionIncapacidades.Core;
using WebServicesGestionIncapacidades.Models.Class.Request;

namespace WebServicesGestionIncapacidades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        public IConfiguration _configuration;
        public FileController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost("uploafile")]
        public IActionResult UploadFile([FromForm] UploadFileRequest uploadFileRequest, [FromHeader] string Token)
        {
            FileCore fileCore = new FileCore(_configuration);
            try
            {
                var result = fileCore.UploadFile(uploadFileRequest, Token);
                switch (result)
                {
                    case "200":
                        return Ok(new { status = "uploaded", uploadFileRequest.Order });
                    case "204":
                        return StatusCode(204, new { status = "204", uploadFileRequest.Order }); // session id no existe
                    case "406":
                        return StatusCode(406, new { status = "406", uploadFileRequest.Order });
                    case "404":
                        return BadRequest("Archivo inválido");
                    default:
                        return BadRequest("Error al subir el archivo");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
