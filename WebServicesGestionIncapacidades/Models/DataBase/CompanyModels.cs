using Microsoft.Data.SqlClient;
using System.Data;
using WebServicesGestionIncapacidades.Models.DataBase.Utilities;

namespace WebServicesGestionIncapacidades.Models.DataBase
{
    public class CompanyModels
    {
        private ConnectionSQLModel dbConnection;
        private readonly IConfiguration _configuration;

        public CompanyModels(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public DataTable GetDataCompany(string nit)
        {
            try
            {
                dbConnection = new ConnectionSQLModel(_configuration);

                List<SqlParameter> parameters = new()
                {
                    dbConnection.CreateParam("nit", nit, DbType.String)
                };
                return dbConnection.GetDataTable("ConsultarDatosEmpresa", parameters);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
