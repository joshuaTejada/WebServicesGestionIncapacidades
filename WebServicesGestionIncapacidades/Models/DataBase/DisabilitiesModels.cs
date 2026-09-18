using Microsoft.Data.SqlClient;
using System.Data;
using WebServicesGestionIncapacidades.Models.Class.Request;
using WebServicesGestionIncapacidades.Models.DataBase.Utilities;

namespace WebServicesGestionIncapacidades.Models.DataBase
{
    public class DisabilitiesModels
    {
        private ConnectionSQLModel dbConnection;
        private readonly IConfiguration _configuration;

        public DisabilitiesModels(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public DataTable PostDisabilities(DisabilitiesRequest disabilitiesRequest)
        {
            try
            {
                dbConnection = new ConnectionSQLModel(_configuration);

                List<SqlParameter> parameters = new()
                {
                    dbConnection.CreateParam("identificacion", disabilitiesRequest.ID, DbType.String),
                    dbConnection.CreateParam("es_transcrita", disabilitiesRequest.Prescriptions, DbType.Boolean),
                    dbConnection.CreateParam("es_prorroga", disabilitiesRequest.Extension, DbType.Boolean),
                    dbConnection.CreateParam("es_transito", disabilitiesRequest.Transit, DbType.Boolean),
                    dbConnection.CreateParam("incapacidad_inicial", disabilitiesRequest.InitialDisability, DbType.String),
                    dbConnection.CreateParam("id_tipo_incapacidad", disabilitiesRequest.IdTypeDisability, DbType.Int64),
                    dbConnection.CreateParam("Fondo_salud", disabilitiesRequest.companyHealth, DbType.String),
                    dbConnection.CreateParam("numero_incapacidad", disabilitiesRequest.NumberDisability, DbType.String),
                    dbConnection.CreateParam("diagnostico", disabilitiesRequest.Diagnosis, DbType.String),
                    dbConnection.CreateParam("desc_diagnostico", disabilitiesRequest.DescriptionDiagnosis, DbType.String),
                    dbConnection.CreateParam("fecha_inicial", disabilitiesRequest.InitialDate, DbType.String),
                    dbConnection.CreateParam("dias", disabilitiesRequest.Day, DbType.Int64),
                    dbConnection.CreateParam("IdTipoDocumentoAdjunto", disabilitiesRequest.IdAttached1, DbType.String),
                    dbConnection.CreateParam("IdTipoDocumentoAdjunto1", disabilitiesRequest.IdAttached2, DbType.String),
                    dbConnection.CreateParam("IdTipoDocumentoAdjunto2", disabilitiesRequest.IdAttached3, DbType.String),
                    dbConnection.CreateParam("IdTipoDocumentoAdjunto3", disabilitiesRequest.IdAttached4, DbType.String),
                    dbConnection.CreateParam("IdTipoDocumentoAdjunto4", disabilitiesRequest.IdAttached5, DbType.String),
                    dbConnection.CreateParam("IdTipoDocumentoAdjunto5", disabilitiesRequest.IdAttached6, DbType.String)
                };
                return dbConnection.GetDataTable("RegistarIncapacidad", parameters);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable GetAttachedForTypeDesability(int IdTypeDisabilities, string IdEPS, int Transcribed, int Transit, string DiagnosticoCode)
        {
            try
            {
                dbConnection = new ConnectionSQLModel(_configuration);

                List<SqlParameter> parameters = new()
                {
                    dbConnection.CreateParam("IdFondo", IdEPS, DbType.String),
                    dbConnection.CreateParam("IdIncapacidades", IdTypeDisabilities, DbType.Int64),
                    dbConnection.CreateParam("Transcrita", Transcribed, DbType.Int16),
                    dbConnection.CreateParam("es_transito ", Transit, DbType.Int16),
                    dbConnection.CreateParam("DiagnosticoCode ", DiagnosticoCode, DbType.String)
                };
                return dbConnection.GetDataTable("ConsultarAdjuntosRequridos", parameters);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable GetDiagnosis(string ValueFind)
        {
            try
            {
                dbConnection = new ConnectionSQLModel(_configuration);

                List<SqlParameter> parameters = new()
                {
                    dbConnection.CreateParam("ValorBusqueda", ValueFind, DbType.String)
                };
                return dbConnection.GetDataTable("ConsultarDiagnosticos", parameters);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable GetFixDesability(int Id)
        {
            try
            {
                dbConnection = new ConnectionSQLModel(_configuration);

                List<SqlParameter> parameters = new()
                {
                    dbConnection.CreateParam("id_incapacidad", Id, DbType.Int64)
                };
                return dbConnection.GetDataTable("ConsultarCorrecionIncapacidad", parameters);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable GetHealthFund(int IdTypeDisability, string empresa, string ID)
        {
            try
            {
                dbConnection = new ConnectionSQLModel(_configuration);

                List<SqlParameter> parameters = new()
                {
                    dbConnection.CreateParam("IdTipoIncapacidades", IdTypeDisability, DbType.Int64),
                    dbConnection.CreateParam("Empresa", empresa, DbType.String),
                    dbConnection.CreateParam("ID", ID, DbType.String)
                };
                return dbConnection.GetDataTable("ConsultarFondoPorTipoIncapacidad", parameters);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable PutDisabilities(int idDisability, int option)
        {
            try
            {
                dbConnection = new ConnectionSQLModel(_configuration);

                List<SqlParameter> parameters = new()
                {
                    dbConnection.CreateParam("id_incapacidad", idDisability, DbType.String),
                    dbConnection.CreateParam("Option", option, DbType.Int64)
                };
                return dbConnection.GetDataTable("ActualizarIncapacidad", parameters);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable GetDisabilities(int ID)
        {
            try
            {
                dbConnection = new ConnectionSQLModel(_configuration);

                List<SqlParameter> parameters = new()
                {
                    dbConnection.CreateParam("Identificacion", ID, DbType.String)
                };
                return dbConnection.GetDataTable("ConsultarIncapacidades", parameters);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable PostLogs(string msj, string identificacion)
        {
            try
            {
                dbConnection = new ConnectionSQLModel(_configuration);

                List<SqlParameter> parameters = new()
                {
                    dbConnection.CreateParam("Datos", msj, DbType.String),
                    dbConnection.CreateParam("identificacion", identificacion, DbType.String)
                };
                return dbConnection.GetDataTable("RegistarLogs", parameters);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
