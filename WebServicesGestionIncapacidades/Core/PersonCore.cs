using System.Data;
using WebServicesGestionIncapacidades.Core.Security;
using WebServicesGestionIncapacidades.Models.Class;
using WebServicesGestionIncapacidades.Models.Class.Request;
using WebServicesGestionIncapacidades.Models.Class.Response;
using WebServicesGestionIncapacidades.Models.DataBase;
using WebServicesGestionIncapacidades.Models.DataBase.Utilities;

namespace WebServicesGestionIncapacidades.Core
{
    public class PersonCore
    {
        private readonly IConfiguration _configuration;

        public PersonCore(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public ResponseModels ValidatePerson(string Token, PersonRequest personRequest)
        {
            ResponseModels responseModels = new();
            try
            {
                responseModels.MessageResponse = "Token expirado";
                responseModels.CodeResponse = "401";

                SecurityCore securityCore1 = new(_configuration);
                var (isValid, claimsPrincipal) = securityCore1.IsTokenValid(Token);
                //isValid = true;
                if (isValid)
                {
                    PersonModels userModel = new(_configuration);
                    Utilities utilities = new(_configuration);
                    DataTable dataUser = userModel.ValidateDataPerson(personRequest);

                    responseModels.MessageResponse = dataUser.Rows[0]["msg"].ToString();
                    responseModels.Data = GetDataGeneralClass(dataUser);
                    if (dataUser.Rows[0]["code"].ToString() == "1")
                    {
                        responseModels.Token = Token;
                        responseModels.CodeResponse = "200";
                    }
                    else
                        responseModels.CodeResponse = "204";
                }
            }
            catch (Exception)
            {
                responseModels.MessageResponse = "Error al validar el usuario";
                responseModels.CodeResponse = "500";
            }
            return responseModels;
        }
        private DataGeneralClass GetDataGeneralClass(DataTable dataUser)
        {
            DataGeneralClass dataGeneralClass = new DataGeneralClass()
            {
                IdEps = dataUser.Rows[0]["IdFondo"].ToString(),
                DataFondos = dataUser.Rows[0]["DataFondos"].ToString(),
                DataTypeDesabilities = dataUser.Rows[0]["DataTipoIncapacidad"].ToString(),
                DataDiagnostico = dataUser.Rows[0]["DataDiagnosticos"].ToString(),
                DataEmpleado = dataUser.Rows[0]["DataEmpleado"].ToString(),
            };
            return dataGeneralClass;
        }
        public ResponseModels PostPerson(string Token, PersonRequest personRequest)
        {
            ResponseModels responseModels = new();
            try
            {
                responseModels.MessageResponse = "Token expirado";
                responseModels.CodeResponse = "401";

                SecurityCore securityCore1 = new(_configuration);
                var (isValid, claimsPrincipal) = securityCore1.IsTokenValid(Token);
                if (isValid)
                {
                    PersonModels userModel = new(_configuration);
                    Utilities utilities = new(_configuration);
                    DataTable dataUser = userModel.PostPerson(personRequest);

                    responseModels.MessageResponse = dataUser.Rows[0]["msg"].ToString();
                    if (dataUser.Rows[0]["code"].ToString() == "1")
                    {
                        responseModels.Token = Token;
                        responseModels.CodeResponse = "201";
                        responseModels.Data = GetDataGeneralClass(dataUser);
                    }
                    else
                        responseModels.CodeResponse = "204";
                }
            }
            catch (Exception)
            {
                responseModels.MessageResponse = "Error al envio de datos";
                responseModels.CodeResponse = "500";
            }
            return responseModels;
        }
    }
}