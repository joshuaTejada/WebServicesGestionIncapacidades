using System.Data;
using WebServicesGestionIncapacidades.Core.Security;
using WebServicesGestionIncapacidades.Models.Class.Request;
using WebServicesGestionIncapacidades.Models.Class.Response;
using WebServicesGestionIncapacidades.Models.DataBase.Utilities;
using WebServicesGestionIncapacidades.Models.DataBase;
using Microsoft.IdentityModel.Tokens;
using WebServicesGestionIncapacidades.Models.Class;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace WebServicesGestionIncapacidades.Core
{
    public class CompanyCore
    {
        private readonly IConfiguration _configuration;

        public CompanyCore(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public CompanyDataResponse GetDataCompany(string TokenCompany)
        {
            CompanyDataResponse responseModels = new();
            try
            {
                responseModels.MessageResponse = "Tu sesión caducó. Recarga la página o vuelve al inicio para continuar";
                responseModels.CodeResponse = "401";

                var nit = IsTokenValid(TokenCompany);

                if (!nit.IsNullOrEmpty())
               {
                    CompanyModels companyModels = new(_configuration);
                    Utilities utilities = new(_configuration);
                    SecurityCore securityCore = new(_configuration);
                    DataTable dataUser = companyModels.GetDataCompany(nit);

                    responseModels.MessageResponse = dataUser.Rows[0]["msg"].ToString();
                    if (dataUser.Rows[0]["code"].ToString() == "1")
                    {
                        responseModels.Token = securityCore.GenerateToken(nit, "");
                        responseModels.CodeResponse = "200";
                        responseModels.Data = GetDataCompanyFormat(dataUser);
                        responseModels.Data.logo = GetLogoBase64(dataUser.Rows[0]["logo"].ToString());
                        responseModels.Data.Background = GetLogoBase64(dataUser.Rows[0]["backgroundo"].ToString());
                    }
                }
            }
            catch (Exception)
            {
                responseModels = new();
                responseModels.MessageResponse = "Error al validar el usuario";
                responseModels.CodeResponse = "500";
            }
            return responseModels;
        }
        public DataCompanyClass GetDataCompanyFormat(DataTable dataUser)
        {
            DataCompanyClass dataCompanyClass = new DataCompanyClass()
            {
                colorPrimario = dataUser.Rows[0]["colorPrimario"].ToString(),
                colorSecundario = dataUser.Rows[0]["colorSecundario"].ToString(),
                colorTerciario = dataUser.Rows[0]["colorTerciario"].ToString(),
                DataTypeID = dataUser.Rows[0]["DataTipoId"].ToString(),
                logo = dataUser.Rows[0]["logo"].ToString()
            };
            return dataCompanyClass;
        }
        public string GetLogoBase64(string pathLogo)
        {        
            string pathClient = _configuration["route:pathLogo"] + "\\" + pathLogo;
            byte[] pdfBytes = System.IO.File.ReadAllBytes(pathClient);
            string base64pdf = Convert.ToBase64String(pdfBytes);
            return base64pdf;
        }
        public string IsTokenValid(string TokenCompany)
        {
            try
            {
                if (string.IsNullOrEmpty(TokenCompany)) return null;

                var key = new byte[32];
                key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);

                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.Zero
                };

                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(TokenCompany, validationParameters, out securityToken);

                return principal.FindFirst(ClaimTypes.Name)?.Value;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
