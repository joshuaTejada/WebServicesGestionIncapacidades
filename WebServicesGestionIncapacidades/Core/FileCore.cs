using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Layout.Borders;
using System.Security.Claims;
using WebServicesGestionIncapacidades.Core.Security;
using WebServicesGestionIncapacidades.Models.Class.Request;
using WebServicesGestionIncapacidades.Models.DataBase;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace WebServicesGestionIncapacidades.Core
{
    public class FileCore
    {
        private readonly IConfiguration _configuration;

        public FileCore(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateSession(string Token)
        {
            string sessionId = null;
            try
            {
                SecurityCore securityCore1 = new(_configuration);
                var (isValid, claimsPrincipal) = securityCore1.IsTokenValid(Token);
                if (isValid)
                {
                    sessionId = $"inc_{Guid.NewGuid():N}";
                    var path = Path.Combine(_configuration["route:pathTemp"], sessionId);

                    Directory.CreateDirectory(path);

                    DisabilitiesModels disabilitiesModels = new(_configuration);
                    disabilitiesModels.PostLogs($"Se ha creado la session temporal {sessionId}", claimsPrincipal.Identity.Name);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return sessionId;
        }

        public string UploadFile(UploadFileRequest uploadFileRequest, string Token)
        {
            try
            {
                SecurityCore securityCore = new(_configuration);
                var (isValid, claimsPrincipal) = securityCore.IsTokenValid(Token);

                if (!isValid)
                    return "401";

                var idToken = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
                if (uploadFileRequest.Identificacion != idToken)
                    return "403";

                if (uploadFileRequest.File == null || uploadFileRequest.File.Length == 0)
                    return "404";

                var sessionPath = Path.Combine(_configuration["route:pathTemp"],uploadFileRequest.SessionId);

                if (!Directory.Exists(sessionPath))
                    return "204";

                // Leer archivo en memoria
                byte[] fileBytes;
                using (var ms = new MemoryStream())
                {
                    uploadFileRequest.File.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }

                // Validar si es PDF
                var extension = Path.GetExtension(uploadFileRequest.File.FileName).ToLower();
                if (extension == ".pdf")
                {
                    if (IsPdfPasswordProtected(fileBytes))
                        return "406"; // PDF protegido
                }

                // Guardar archivo
                var fileName = $"{uploadFileRequest.Order:D4}{extension}";
                var fullPath = Path.Combine(sessionPath, fileName);

                using var fs = new FileStream(fullPath, FileMode.Create);
                fs.Write(fileBytes, 0, fileBytes.Length);

                return "200";
            }
            catch
            {
                return "500";
            }
        }
        private bool IsPdfPasswordProtected(byte[] pdfBytes)
        {
            try
            {
                using var reader = new PdfReader(new MemoryStream(pdfBytes));
                using var pdfDoc = new PdfDocument(reader);
                return false;
            }
            catch (BadPasswordException)
            {
                return true;
            }
        }
    }
}
