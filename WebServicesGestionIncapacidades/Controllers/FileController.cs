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
        [HttpPost("uploadfile")]
        public IActionResult UploadFile([FromForm] UploadFileRequest uploadFileRequest, [FromHeader] string Token)
        {
            FileCore fileCore = new FileCore(_configuration);
            try
            {
                var result = fileCore.UploadFile(uploadFileRequest, Token);
                switch (result)
                {
                    case "200":
                        return Ok(new { status = "200", uploadFileRequest.Order });
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
        [HttpDelete("deletefile")]
        public IActionResult DeleteFile([FromForm] int order,[FromForm] string sessionId,[FromHeader] string Token)
        {
            try
            {
                FileCore fileCore = new FileCore(_configuration);
                var result = fileCore.DeleteFile(sessionId, order, Token);

                return result switch
                {
                    "200" => Ok(new
                    {
                        status = 200,
                        message = "Archivo eliminado correctamente."
                    }),

                    "400" => BadRequest(new
                    {
                        status = 400,
                        message = "Parámetros inválidos. Verifique el order y el sessionId."
                    }),

                    "401" => Unauthorized(new
                    {
                        status = 401,
                        message = "Token inválido o expirado."
                    }),

                    "403" => StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        status = 403,
                        message = "No tiene permisos para eliminar archivos de esta sesión."
                    }),

                    "404" => NotFound(new
                    {
                        status = 404,
                        message = "El archivo solicitado no existe."
                    }),

                    "204" => NoContent(),

                    "409" => Conflict(new
                    {
                        status = 409,
                        message = "No fue posible eliminar el archivo debido a un conflicto en el sistema."
                    }),

                    _ => StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        status = 500,
                        message = "Error interno al eliminar el archivo."
                    })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    status = 500,
                    message = "Excepción no controlada en el servicio.",
                    detail = ex.Message
                });
            }
        }
    }
}