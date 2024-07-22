using Microsoft.Data.SqlClient;
using System.Data;
using WebServicesGestionIncapacidades.Models.Class.Request;
using WebServicesGestionIncapacidades.Models.DataBase.Utilities;

namespace WebServicesGestionIncapacidades.Models.DataBase
{
    public class PersonModels
    {
        private ConnectionSQLModel dbConnection;
        private readonly IConfiguration _configuration;

        public PersonModels(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public DataTable ValidateDataPerson(PersonRequest userRequest)
        {
            try
            {
                dbConnection = new ConnectionSQLModel(_configuration);

                List<SqlParameter> parameters = new()
                {
                    dbConnection.CreateParam("TipoIdentificacion", userRequest.TypeId, DbType.String),
                    dbConnection.CreateParam("ID", userRequest.ID, DbType.String),
                    dbConnection.CreateParam("Correo", userRequest.Email, DbType.String)
                };
                return dbConnection.GetDataTable("ValidarPersona", parameters);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable PostPerson(PersonRequest personRequest)
        {
            try
            {
                dbConnection = new ConnectionSQLModel(_configuration);

                List<SqlParameter> parameters = new()
                {
                    dbConnection.CreateParam("TipoIdentificacion", personRequest.TypeId, DbType.String),
                    dbConnection.CreateParam("ID", personRequest.ID, DbType.String),
                    dbConnection.CreateParam("Nombre", personRequest.Name, DbType.String),
                    dbConnection.CreateParam("Apellido", personRequest.LastName, DbType.String),
                    dbConnection.CreateParam("Correo", personRequest.Email, DbType.String),
                    dbConnection.CreateParam("Movil", personRequest.Cel, DbType.String),
                    dbConnection.CreateParam("FechaIngreso", personRequest.DateStart, DbType.Date),
                    dbConnection.CreateParam("FondoSalud", personRequest.EPS, DbType.String),
                    dbConnection.CreateParam("Empresa", personRequest.NitCompany, DbType.String)
                };
                return dbConnection.GetDataTable("RegistarPersona", parameters);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}