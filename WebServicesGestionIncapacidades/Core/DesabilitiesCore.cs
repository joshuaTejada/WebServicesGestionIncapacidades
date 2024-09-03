using Newtonsoft.Json;
using System.Data;
using System.Security.Claims;
using WebServicesGestionIncapacidades.Core.Security;
using WebServicesGestionIncapacidades.Models.Class;
using WebServicesGestionIncapacidades.Models.Class.Request;
using WebServicesGestionIncapacidades.Models.Class.Response;
using WebServicesGestionIncapacidades.Models.DataBase;
using WebServicesGestionIncapacidades.Models.DataBase.Utilities;

namespace WebServicesGestionIncapacidades.Core
{
    public class DesabilitiesCore
    {
        private readonly IConfiguration _configuration;

        public DesabilitiesCore(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public AttachedResponse GetAttachedRequired(string Token, int IdTypeDisabilities, string IdEPS, int Transcribed, int Transit, string DiagnosticoCode)
        {
            AttachedResponse responseModels = new();
            try
            {
                responseModels.MessageResponse = "Token expirado";
                responseModels.CodeResponse = "401";

                SecurityCore securityCore1 = new(_configuration);
                var (isValid, claimsPrincipal) = securityCore1.IsTokenValid(Token);
                if (isValid)
                {
                    string nit_cliente = int.Parse(claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value).ToString();
                    DisabilitiesModels disabilitiesModels = new(_configuration);
                    Utilities utilities = new(_configuration);
                    DataTable data = disabilitiesModels.GetAttachedForTypeDesability(IdTypeDisabilities, IdEPS, Transcribed, Transit, DiagnosticoCode);

                    responseModels.MessageResponse = data.Rows[0]["msg"].ToString();
                    if (data.Rows[0]["code"].ToString() == "1")
                    {
                        responseModels.Token = Token;
                        responseModels.CodeResponse = "200";
                        var documentosRequeridos = "[" + data.Rows[0]["DocumentoRequeridos"].ToString() + "]";
                        if (!string.IsNullOrEmpty(documentosRequeridos))
                        {
                            List<AttachedRequiredClass> AttachedRequiredClassList = JsonConvert.DeserializeObject<List<AttachedRequiredClass>>(documentosRequeridos);
                            responseModels.Data = AttachedRequiredClassList;
                        }

                        DataTable dataARL = disabilitiesModels.GetHealthFund(IdTypeDisabilities, nit_cliente);
                        responseModels.IdARL = int.Parse(dataARL.Rows[0]["id_arl"].ToString());
                        responseModels.Base64ImgEps = Transcribed == 1 ? GetEPSBase64(data.Rows[0]["codigo"].ToString()) : GetEPSBase64("no_transcrita");
                    }
                    else
                        responseModels.CodeResponse = "204";
                }
            }
            catch (Exception ex)
            {
                responseModels.MessageResponse = "Error al validar el usuario";
                responseModels.CodeResponse = "500";
            }
            return responseModels;
        }
        public string GetEPSBase64(string path)
        {
            string pathClient = _configuration["route:pathFondos"] + "\\" + path + ".jpg";
            if (!File.Exists(pathClient)) return null;
            byte[] pdfBytes = System.IO.File.ReadAllBytes(pathClient);
            string base64pdf = Convert.ToBase64String(pdfBytes);
            return base64pdf;
        }
        public DiagnosisResponse GetDiagnosis(string Token, string ValueFind)
        {
            DiagnosisResponse responseModels = new();
            try
            {
                responseModels.MessageResponse = "Token expirado";
                responseModels.CodeResponse = "401";

                SecurityCore securityCore1 = new(_configuration);
                var (isValid, claimsPrincipal) = securityCore1.IsTokenValid(Token);
                if (isValid)
                {
                    DisabilitiesModels disabilitiesModels = new(_configuration);
                    Utilities utilities = new(_configuration);
                    DataTable data = disabilitiesModels.GetDiagnosis(ValueFind);

                    responseModels.MessageResponse = data.Rows[0]["msg"].ToString();
                    if (data.Rows[0]["code"].ToString() == "1")
                    {
                        responseModels.Token = Token;
                        responseModels.CodeResponse = "200";
                        var DataDiagnosticos = "[" + data.Rows[0]["DataDiagnosticos"].ToString() + "]";
                        if (!string.IsNullOrEmpty(DataDiagnosticos))
                        {
                            List<DiagnosisClass> AttachedRequiredClassList = JsonConvert.DeserializeObject<List<DiagnosisClass>>(DataDiagnosticos);
                            responseModels.Data = AttachedRequiredClassList;
                        }
                    }
                    else
                        responseModels.CodeResponse = "204";
                }
            }
            catch (Exception ex)
            {
                responseModels.MessageResponse = "Error al buscar los diagnostico";
                responseModels.CodeResponse = "500";
            }
            return responseModels;
        }
        public DisabilityResponse PostDisabilities(string Token, DisabilitiesRequest disabilitiesRequest)
        {
            DisabilityResponse responseModels = new();
            try
            {
                responseModels.MessageResponse = "Token expirado";
                responseModels.CodeResponse = "401";

                SecurityCore securityCore1 = new(_configuration);
                var (isValid, claimsPrincipal) = securityCore1.IsTokenValid(Token);
                if (isValid)
                {
                    DisabilitiesModels disabilitiesModels = new(_configuration);
                    Utilities utilities = new(_configuration);
                    DataTable data = disabilitiesModels.PostDisabilities(disabilitiesRequest);

                    responseModels.MessageResponse = data.Rows[0]["msg"].ToString();
                    if (data.Rows[0]["code"].ToString() == "1")
                    {
                        var attachments = new List<(string base64, string id)>
                        {
                            (disabilitiesRequest.Base64Attached1, disabilitiesRequest.IdAttached1),
                            (disabilitiesRequest.Base64Attached2, disabilitiesRequest.IdAttached2),
                            (disabilitiesRequest.Base64Attached3, disabilitiesRequest.IdAttached3),
                            (disabilitiesRequest.Base64Attached4, disabilitiesRequest.IdAttached4),
                            (disabilitiesRequest.Base64Attached5, disabilitiesRequest.IdAttached5),
                            (disabilitiesRequest.Base64Attached6, disabilitiesRequest.IdAttached6)
                        };
                        string id_incapacidad = data.Rows[0]["id_incapacidad"].ToString();
                        foreach (var (base64, id) in attachments)
                        {
                            if (!string.IsNullOrEmpty(base64))
                            {
                                var (IsValid, Message) = SaveDocument(base64, $"{data.Rows[0]["id_empresa"]}_{id_incapacidad}_{id}.pdf");                                
                            }
                        }

                        // Notificar al usuario de su incapacidad
                        UtilitiesCore utilitiesCore = new(_configuration);
                        utilitiesCore.SendEmail(data.Rows[0]["email"].ToString(), "Incapacidad recibida", getBodyEmail(data), true);

                        responseModels.Token = Token;
                        responseModels.CodeResponse = "201";
                        responseModels.Data = id_incapacidad;
                    }
                    else
                        responseModels.CodeResponse = "200";
                }
            }
            catch (Exception ex)
            {
                responseModels.MessageResponse = "Error al guardar la incapacidad";
                responseModels.CodeResponse = "500";
            }
            return responseModels;
        }
        public (bool IsValid, string Message) SaveDocument(string? Base64Document, string? fileName)
        {
            try
            {
                // Decodificar el pdf base64
                byte[] documentBytes = Convert.FromBase64String(Base64Document);

                // Ruta donde se guardan los adjuntos
                string pathDocument = _configuration["route:pathDocument"];
                string imagePath = Path.Combine(pathDocument, "", fileName);

                if (File.Exists(imagePath)) File.Delete(imagePath);

                // Guardar la imagen en el servidor
                System.IO.File.WriteAllBytes(imagePath, documentBytes);

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public string getBodyEmail(DataTable dataUser)
        {
            string? color_primario = dataUser.Rows[0]["color_primario"].ToString();
            string? color_secundario = dataUser.Rows[0]["color_secundario"].ToString();
            string? color_terciario = dataUser.Rows[0]["color_terciario"].ToString();
            string? logo = dataUser.Rows[0]["logo"].ToString();
            string? id_radicado = dataUser.Rows[0]["id_incapacidad"].ToString();

            return $"<!DOCTYPE HTML PUBLIC '-//W3C//DTD XHTML 1.0 Transitional //EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>\r\n<html xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office'>\r\n<head>\r\n  <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>\r\n  <meta name='viewport' content='width=device-width, initial-scale=1.0'>\r\n  <meta name='x-apple-disable-message-reformatting'>\r\n  <meta http-equiv='X-UA-Compatible' content='IE=edge'>\r\n  <title></title>\r\n  <style type='text/css'>\r\n    @media only screen and (min-width: 520px) {{\r\n      .u-row {{\r\n        width: 500px !important;\r\n      }}\r\n      .u-row .u-col {{\r\n        vertical-align: top;\r\n      }}\r\n      .u-row .u-col-100 {{\r\n        width: 500px !important;\r\n      }}\r\n    }}\r\n    \r\n    @media (max-width: 520px) {{\r\n      .u-row-container {{\r\n        max-width: 100% !important;\r\n        padding-left: 0px !important;\r\n        padding-right: 0px !important;\r\n      }}\r\n      .u-row .u-col {{\r\n        min-width: 320px !important;\r\n        max-width: 100% !important;\r\n        display: block !important;\r\n      }}\r\n      .u-row {{\r\n        width: 100% !important;\r\n      }}\r\n      .u-col {{\r\n        width: 100% !important;\r\n      }}\r\n      .u-col>div {{\r\n        margin: 0 auto;\r\n      }}\r\n    }}\r\n    \r\n    body {{\r\n      margin: 0;\r\n      padding: 0;\r\n    }}\r\n    \r\n    table,\r\n    tr,\r\n    td {{\r\n      vertical-align: top;\r\n      border-collapse: collapse;\r\n    }}\r\n    \r\n    p {{\r\n      margin: 0;\r\n    }}\r\n    \r\n    .ie-container table,\r\n    .mso-container table {{\r\n      table-layout: fixed;\r\n    }}\r\n    \r\n    * {{\r\n      line-height: inherit;\r\n    }}\r\n    \r\n    a[x-apple-data-detectors='true'] {{\r\n      color: inherit !important;\r\n      text-decoration: none !important;\r\n    }}\r\n    \r\n    table,\r\n    td {{\r\n      color: #000000;\r\n    }}\r\n  </style>\r\n</head>\r\n<body class='clean-body u_body' style='margin: 0;padding: 0;-webkit-text-size-adjust: 100%;background-color: #e7e7e7;color: #000000'>\r\n  <table style='border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;min-width: 320px;Margin: 0 auto;background-color: #e7e7e7;width:100%' cellpadding='0' cellspacing='0'>\r\n    <tbody>\r\n      <tr style='vertical-align: top'>\r\n        <td style='word-break: break-word;border-collapse: collapse !important;vertical-align: top'>\r\n          <div class='u-row-container' style='padding: 0px;background-color: transparent'>\r\n            <div class='u-row' style='margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>\r\n              <div style='border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;'>\r\n                <div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>\r\n                  <div style='background-color: {color_primario};height: 100%;width: 100% !important;'>\r\n                    <div style='box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'>\r\n                      <table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>\r\n                        <tbody>\r\n                          <tr>\r\n                            <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>\r\n                              <table width='100%' cellpadding='0' cellspacing='0' border='0'>\r\n                                <tr style=\"vertical-align: middle;\">\r\n                                  <td style='padding-right: 0px;padding-left: 0px;' align='center'>\r\n                                    <img align='center' border='0' src='https://sigha.com.co/incapacidades/_lib/img/grp__NM__img__NM__logo_incapacidades_blanco.png' alt='' title='' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 100%;max-width: 130px;'\r\n                                      width='130' />\r\n                                  </td>\r\n                                  <td style='padding-right: 0px;padding-left: 0px;' align='center'>\r\n                                    <img align='center' border='0' src='https://sigha.com.co/incapacidades/_lib/img/{logo}' alt='' title='' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 100%;max-width: 130px;'\r\n                                      width='130' />\r\n                                  </td>\r\n                                </tr>\r\n                              </table>\r\n                            </td>\r\n                          </tr>\r\n                        </tbody>\r\n                      </table>\r\n                    </div>\r\n                  </div>\r\n                </div>\r\n              </div>\r\n            </div>\r\n          </div>\r\n          <div class='u-row-container' style='padding: 0px;background-color: transparent'>\r\n            <div class='u-row' style='margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>\r\n              <div style='border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;'>\r\n                <div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>\r\n                  <div style='background-color: #efefef;height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;'>\r\n                    <div style='box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;'>\r\n                      <table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>\r\n                        <tbody>\r\n                          <tr>\r\n                            <td style='overflow-wrap:break-word;word-break:break-word;padding:32px 0px 0px;font-family:arial,helvetica,sans-serif;' align='left'>\r\n                              <h1 style='margin: 0px; line-height: 140%; text-align: center; word-wrap: break-word; font-size: 22px; font-weight: 400;'><span><span><span><strong>Incapacidad enviada</strong></span></span>\r\n                                </span>\r\n                              </h1>\r\n                            </td>\r\n                          </tr>\r\n                        </tbody>\r\n                      </table>\r\n                    </div>\r\n                  </div>\r\n                </div>\r\n              </div>\r\n            </div>\r\n          </div>\r\n          <div class='u-row-container' style='padding: 0px;background-color: transparent'>\r\n            <div class='u-row' style='margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>\r\n              <div style='border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;'>\r\n                <div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>\r\n                  <div style='background-color: #efefef;height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;'>\r\n                    <div style='box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;'>\r\n                      <table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>\r\n                        <tbody>\r\n                          <tr>\r\n                            <td style='overflow-wrap:break-word;word-break:break-word;padding:24px;font-family:arial,helvetica,sans-serif;' align='left'>\r\n                              <table width='100%' cellpadding='0' cellspacing='0' border='0'>\r\n                                <tr>\r\n                                  <td style='padding-right: 0px;padding-left: 0px;' align='center'>\r\n                                    <img align='center' border='0' src='https://assets.unlayer.com/projects/240289/1720042545222-incapacidadEnviada.png' alt='' title='' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 100%;max-width: 132px;'\r\n                                      width='132' />\r\n                                  </td>\r\n                                </tr>\r\n                              </table>\r\n                            </td>\r\n                          </tr>\r\n                        </tbody>\r\n                      </table>\r\n                    </div>\r\n                  </div>\r\n                </div>\r\n              </div>\r\n            </div>\r\n          </div>\r\n          <div class='u-row-container' style='padding: 0px;background-color: transparent'>\r\n            <div class='u-row' style='margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>\r\n              <div style='border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;'>\r\n                <div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>\r\n                  <div style='background-color: #efefef;height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;'>\r\n                    <div style='box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;'>\r\n                      <table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>\r\n                        <tbody>\r\n                          <tr>\r\n                            <td style='overflow-wrap:break-word;word-break:break-word;padding:0px 32px 32px;font-family:arial,helvetica,sans-serif;' align='left'>\r\n                              <div style='font-size: 14px; color: #000000; line-height: 140%; text-align: center; word-wrap: break-word;'>\r\n                                <p style='line-height: 140%;'><span data-metadata=''\r\n                                    style='line-height: 19.6px;'></span>Tu incapacidad ha sido registrada exitosamente, en caso de presentar alguna novedad se te notificara por este mismo medio.</p>\r\n <div style='margin-top: 16px; text-align: center; word-wrap: break-word; background-color: {color_primario}; border-radius: 4px; padding: 12px 24px;'>\r\n <p style='color: #ffffff; line-height: 140%;'>ID: {id_radicado}</p>\r\n</div>\r\n                              </div>\r\n                            </td>\r\n                          </tr>\r\n                        </tbody>\r\n                      </table>\r\n                    </div>\r\n                  </div>\r\n                </div>\r\n              </div>\r\n            </div>\r\n          </div>\r\n          <div class='u-row-container' style='padding: 0px;background-color: transparent'>\r\n            <div class='u-row' style='margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>\r\n              <div style='border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;'>\r\n                <div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>\r\n                  <div style='background-color: {color_terciario};height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;'>\r\n                    <div style='box-sizing: border-box; height: 100%; padding: 32px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;'>\r\n                      <table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>\r\n                        <tbody>\r\n                          <tr>\r\n                            <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>\r\n\r\n                              <div style='font-size: 12px; line-height: 140%; text-align: center; word-wrap: break-word;'>\r\n                                <p style='line-height: 140%;'><strong>ESTA ES UNA CUENTA AUTOMÁTICA PARA ENVÍO DE INFORMACIÓN.</strong></p>\r\n                                <p style='line-height: 140%;'>Por favor NO responda este correo ni escriba a esta dirección. </p>\r\n                              </div>\r\n\r\n                            </td>\r\n                          </tr>\r\n                        </tbody>\r\n                      </table>\r\n                    </div>\r\n                  </div>\r\n                </div>\r\n              </div>\r\n            </div>\r\n          </div>\r\n        </td>\r\n      </tr>\r\n    </tbody>\r\n  </table>\r\n</body>\r\n</html>";
            //return $"<!DOCTYPE HTML PUBLIC '-//W3C//DTD XHTML 1.0 Transitional //EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>\r\n<html xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office'>\r\n<head>\r\n  <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>\r\n  <meta name='viewport' content='width=device-width, initial-scale=1.0'>\r\n  <meta name='x-apple-disable-message-reformatting'>\r\n  <meta http-equiv='X-UA-Compatible' content='IE=edge'>\r\n  <title></title>\r\n  <style type='text/css'>\r\n    @media only screen and (min-width: 520px) {{\r\n      .u-row {{\r\n        width: 500px !important;\r\n      }}\r\n      .u-row .u-col {{\r\n        vertical-align: top;\r\n      }}\r\n      .u-row .u-col-100 {{\r\n        width: 500px !important;\r\n      }}\r\n    }}\r\n    \r\n    @media (max-width: 520px) {{\r\n      .u-row-container {{\r\n        max-width: 100% !important;\r\n        padding-left: 0px !important;\r\n        padding-right: 0px !important;\r\n      }}\r\n      .u-row .u-col {{\r\n        min-width: 320px !important;\r\n        max-width: 100% !important;\r\n        display: block !important;\r\n      }}\r\n      .u-row {{\r\n        width: 100% !important;\r\n      }}\r\n      .u-col {{\r\n        width: 100% !important;\r\n      }}\r\n      .u-col>div {{\r\n        margin: 0 auto;\r\n      }}\r\n    }}\r\n    \r\n    body {{\r\n      margin: 0;\r\n      padding: 0;\r\n    }}\r\n    \r\n    table,\r\n    tr,\r\n    td {{\r\n      vertical-align: top;\r\n      border-collapse: collapse;\r\n    }}\r\n    \r\n    p {{\r\n      margin: 0;\r\n    }}\r\n    \r\n    .ie-container table,\r\n    .mso-container table {{\r\n      table-layout: fixed;\r\n    }}\r\n    \r\n    * {{\r\n      line-height: inherit;\r\n    }}\r\n    \r\n    a[x-apple-data-detectors='true'] {{\r\n      color: inherit !important;\r\n      text-decoration: none !important;\r\n    }}\r\n    \r\n    table,\r\n    td {{\r\n      color: #000000;\r\n    }}\r\n  </style>\r\n</head>\r\n<body class='clean-body u_body' style='margin: 0;padding: 0;-webkit-text-size-adjust: 100%;background-color: #e7e7e7;color: #000000'>\r\n  <table style='border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;min-width: 320px;Margin: 0 auto;background-color: #e7e7e7;width:100%' cellpadding='0' cellspacing='0'>\r\n    <tbody>\r\n      <tr style='vertical-align: top'>\r\n        <td style='word-break: break-word;border-collapse: collapse !important;vertical-align: top'>\r\n          <div class='u-row-container' style='padding: 0px;background-color: transparent'>\r\n            <div class='u-row' style='margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>\r\n              <div style='border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;'>\r\n                <div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>\r\n                  <div style='background-color: {color_primario};height: 100%;width: 100% !important;'>\r\n                    <div style='box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;'>\r\n                      <table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>\r\n                        <tbody>\r\n                          <tr>\r\n                            <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>\r\n                              <table width='100%' cellpadding='0' cellspacing='0' border='0'>\r\n                                <tr style=\"vertical-align: middle;\">\r\n                                  <td style='padding-right: 0px;padding-left: 0px;' align='center'>\r\n                                    <img align='center' border='0' src='http://10.100.10.21/test/gestion_incapacidades/_lib/img/grp__NM__img__NM__logo_incapacidades_blanco.png' alt='' title='' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 100%;max-width: 130px;'\r\n                                      width='130' />\r\n                                  </td>\r\n                                  <td style='padding-right: 0px;padding-left: 0px;' align='center'>\r\n                                    <img align='center' border='0' src='http://10.100.10.21/test/gestion_incapacidades/_lib/img/{logo}' alt='' title='' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 100%;max-width: 130px;'\r\n                                      width='130' />\r\n                                  </td>\r\n                                </tr>\r\n                              </table>\r\n                            </td>\r\n                          </tr>\r\n                        </tbody>\r\n                      </table>\r\n                    </div>\r\n                  </div>\r\n                </div>\r\n              </div>\r\n            </div>\r\n          </div>\r\n          <div class='u-row-container' style='padding: 0px;background-color: transparent'>\r\n            <div class='u-row' style='margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>\r\n              <div style='border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;'>\r\n                <div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>\r\n                  <div style='background-color: #efefef;height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;'>\r\n                    <div style='box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;'>\r\n                      <table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>\r\n                        <tbody>\r\n                          <tr>\r\n                            <td style='overflow-wrap:break-word;word-break:break-word;padding:32px 0px 0px;font-family:arial,helvetica,sans-serif;' align='left'>\r\n                              <h1 style='margin: 0px; line-height: 140%; text-align: center; word-wrap: break-word; font-size: 22px; font-weight: 400;'><span><span><span><strong>Incapacidad enviada</strong></span></span>\r\n                                </span>\r\n                              </h1>\r\n                            </td>\r\n                          </tr>\r\n                        </tbody>\r\n                      </table>\r\n                    </div>\r\n                  </div>\r\n                </div>\r\n              </div>\r\n            </div>\r\n          </div>\r\n          <div class='u-row-container' style='padding: 0px;background-color: transparent'>\r\n            <div class='u-row' style='margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>\r\n              <div style='border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;'>\r\n                <div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>\r\n                  <div style='background-color: #efefef;height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;'>\r\n                    <div style='box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;'>\r\n                      <table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>\r\n                        <tbody>\r\n                          <tr>\r\n                            <td style='overflow-wrap:break-word;word-break:break-word;padding:24px;font-family:arial,helvetica,sans-serif;' align='left'>\r\n                              <table width='100%' cellpadding='0' cellspacing='0' border='0'>\r\n                                <tr>\r\n                                  <td style='padding-right: 0px;padding-left: 0px;' align='center'>\r\n                                    <img align='center' border='0' src='https://assets.unlayer.com/projects/240289/1720042545222-incapacidadEnviada.png' alt='' title='' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 100%;max-width: 132px;'\r\n                                      width='132' />\r\n                                  </td>\r\n                                </tr>\r\n                              </table>\r\n                            </td>\r\n                          </tr>\r\n                        </tbody>\r\n                      </table>\r\n                    </div>\r\n                  </div>\r\n                </div>\r\n              </div>\r\n            </div>\r\n          </div>\r\n          <div class='u-row-container' style='padding: 0px;background-color: transparent'>\r\n            <div class='u-row' style='margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>\r\n              <div style='border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;'>\r\n                <div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>\r\n                  <div style='background-color: #efefef;height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;'>\r\n                    <div style='box-sizing: border-box; height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;'>\r\n                      <table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>\r\n                        <tbody>\r\n                          <tr>\r\n                            <td style='overflow-wrap:break-word;word-break:break-word;padding:0px 32px 32px;font-family:arial,helvetica,sans-serif;' align='left'>\r\n                              <div style='font-size: 14px; color: #000000; line-height: 140%; text-align: center; word-wrap: break-word;'>\r\n                                <p style='line-height: 140%;'><span data-metadata=''\r\n                                    style='line-height: 19.6px;'></span>Tu incapacidad ha sido registrada exitosamente, en caso de presentar alguna novedad se te notificara por este mismo medio.</p>\r\n                              </div>\r\n                            </td>\r\n                          </tr>\r\n                        </tbody>\r\n                      </table>\r\n                    </div>\r\n                  </div>\r\n                </div>\r\n              </div>\r\n            </div>\r\n          </div>\r\n          <div class='u-row-container' style='padding: 0px;background-color: transparent'>\r\n            <div class='u-row' style='margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>\r\n              <div style='border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;'>\r\n                <div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>\r\n                  <div style='background-color: {color_terciario};height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;'>\r\n                    <div style='box-sizing: border-box; height: 100%; padding: 32px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;'>\r\n                      <table style='font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>\r\n                        <tbody>\r\n                          <tr>\r\n                            <td style='overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>\r\n\r\n                              <div style='font-size: 12px; line-height: 140%; text-align: center; word-wrap: break-word;'>\r\n                                <p style='line-height: 140%;'><strong>ESTA ES UNA CUENTA AUTOMÁTICA PARA ENVÍO DE INFORMACIÓN.</strong></p>\r\n                                <p style='line-height: 140%;'>Por favor NO responda este correo ni escriba a esta dirección. </p>\r\n                              </div>\r\n\r\n                            </td>\r\n                          </tr>\r\n                        </tbody>\r\n                      </table>\r\n                    </div>\r\n                  </div>\r\n                </div>\r\n              </div>\r\n            </div>\r\n          </div>\r\n        </td>\r\n      </tr>\r\n    </tbody>\r\n  </table>\r\n</body>\r\n</html>";
        }
        public FixDesabilityResponse GetFixDesability(string Token, int Id)
        {
            FixDesabilityResponse responseModels = new();
            try
            {
                responseModels.MessageResponse = "Token expirado";
                responseModels.CodeResponse = "401";

                SecurityCore securityCore1 = new(_configuration);
                var isValid = IsTokenValidDisability(Token, Id);
                if (isValid)
                {
                    DisabilitiesModels disabilitiesModels = new(_configuration);
                    UtilitiesCore utilitiesCore = new(_configuration);
                    DataTable data = disabilitiesModels.GetFixDesability(Id);

                    responseModels.MessageResponse = data.Rows[0]["msg"].ToString();
                    if (data.Rows[0]["code"].ToString() == "1")
                    {
                        responseModels.Token = Token;
                        responseModels.CodeResponse = "200";
                        var DataIncapacidad = "" + data.Rows[0]["DataIncapacidad"].ToString() + "";
                        var DataCompany = "" + data.Rows[0]["DataCompany"].ToString() + "";
                        var DataNovedadesDocumentos = "[" + data.Rows[0]["DataNovedadesDocumentos"].ToString() + "]";
                        var DataDocumentos = "[" + data.Rows[0]["DataDocumentos"].ToString() + "]";
                        if (!string.IsNullOrEmpty(DataIncapacidad))
                        {
                            DisabilityClass disabilityClass = JsonConvert.DeserializeObject<DisabilityClass>(DataIncapacidad);
                            companyParameterClass companyParameterClass = JsonConvert.DeserializeObject<companyParameterClass>(DataCompany);
                            List<FixDocumentClass> fixDocumentClasses = JsonConvert.DeserializeObject<List<FixDocumentClass>>(DataNovedadesDocumentos);
                            List<DocumentClass> documentClasses = JsonConvert.DeserializeObject<List<DocumentClass>>(DataDocumentos);

                            FixDesalibityClass fixDesalibityClass = new FixDesalibityClass()
                            {
                                disabilityClass = disabilityClass,
                                fixDocumentClass = fixDocumentClasses,
                                documentClass = documentClasses,
                                companyparameter = companyParameterClass,
                                Base64ImgEps = utilitiesCore.GetEPSBase64(data.Rows[0]["CodigoFondo"].ToString()),
                                logo = utilitiesCore.GetLogoBase64(data.Rows[0]["id_empresa"].ToString())
                            };
                            responseModels.Data = fixDesalibityClass;
                        }
                    }
                    else
                        responseModels.CodeResponse = "204";
                }
            }
            catch (Exception ex)
            {
                responseModels.MessageResponse = "Error al buscar la incapacidad";
                responseModels.CodeResponse = "500";
            }
            return responseModels;
        }
        public bool IsTokenValidDisability(string token, int Id)
        {
            try
            {
                if (string.IsNullOrEmpty(token)) return false;

                var key = _configuration["JwtSettings:SaltFixDesability"];
                UtilitiesCore utilitiesCore = new(_configuration);
                if (utilitiesCore.GetSHA512(Id.ToString() + key) == token)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public DisabilityResponse PutDisabilities(string Token, DocumentRequest documentRequest)
        {
            DisabilityResponse responseModels = new();
            try
            {
                responseModels.MessageResponse = "Token expirado";
                responseModels.CodeResponse = "401";

                SecurityCore securityCore1 = new(_configuration);
                var isValid = IsTokenValidDisability(Token, documentRequest.IdDisability);
                if (isValid)
                {
                    DisabilitiesModels disabilitiesModels = new(_configuration);
                    Utilities utilities = new(_configuration);
                    DataTable data = disabilitiesModels.PutDisabilities(documentRequest.IdDisability, 1);

                    responseModels.MessageResponse = data.Rows[0]["msg"].ToString();
                    if (data.Rows[0]["code"].ToString() == "1")
                    {
                        var attachments = new List<(string base64, string id)>
                        {
                            (documentRequest.Base64Attached1, documentRequest.IdAttached1),
                            (documentRequest.Base64Attached2, documentRequest.IdAttached2),
                            (documentRequest.Base64Attached3, documentRequest.IdAttached3),
                            (documentRequest.Base64Attached4, documentRequest.IdAttached4),
                            (documentRequest.Base64Attached5, documentRequest.IdAttached5),
                            (documentRequest.Base64Attached6, documentRequest.IdAttached6)
                        };
                        string DoumentDetails = "los siguientes documentos no se actualizaron: ";

                        foreach (var (base64, id) in attachments)
                        {
                            if (!string.IsNullOrEmpty(base64))
                            {
                                var (IsValid, Message) = SaveDocument(base64, $"{data.Rows[0]["id_empresa"]}_{documentRequest.IdDisability}_{id}.pdf");
                                if (!IsValid)
                                {
                                    DoumentDetails = DoumentDetails + $"({id}) ";
                                };
                            }
                        }

                        responseModels.Token = Token;
                        responseModels.CodeResponse = "201";
                        responseModels.Data = DoumentDetails;
                    }
                    else
                        responseModels.CodeResponse = "200";
                }
            }
            catch (Exception ex)
            {
                responseModels.MessageResponse = "Error al guardar la incapacidad";
                responseModels.CodeResponse = "500";
            }
            return responseModels;
        }
        public healthFundResponse GetHealthFund(string Token, int Id)
        {
            healthFundResponse responseModels = new();
            try
            {
                responseModels.MessageResponse = "Token expirado";
                responseModels.CodeResponse = "401";

                SecurityCore securityCore1 = new(_configuration);
                var (isValid, claimsPrincipal) = securityCore1.IsTokenValid(Token);
                if (isValid)
                {
                    string nit_cliente = int.Parse(claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value).ToString();

                    DisabilitiesModels disabilitiesModels = new(_configuration);
                    DataTable data = disabilitiesModels.GetHealthFund(Id, nit_cliente);

                    responseModels.MessageResponse = data.Rows[0]["msg"].ToString();
                    if (data.Rows[0]["code"].ToString() == "1")
                    {
                        responseModels.Token = Token;
                        responseModels.CodeResponse = "200";
                        responseModels.IdARL = data.Rows[0]["id_arl"].ToString();
                        
                    }
                    else
                        responseModels.CodeResponse = "204";
                }
            }
            catch (Exception ex)
            {
                responseModels.MessageResponse = "Error al buscar los fondos arl - " + ex.Message;
                responseModels.CodeResponse = "500";
            }
            return responseModels;
        }
    }
}
