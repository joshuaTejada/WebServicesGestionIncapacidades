using Microsoft.Data.SqlClient;
using System.Data;

namespace WebServicesGestionIncapacidades.Models.DataBase.Utilities
{
    public class ConnectionSQLModel
    {
        private readonly string Connection;
        public ConnectionSQLModel(IConfiguration configuration)
        {
            Utilities utilities = new Utilities(configuration);
            Connection = utilities.RetornarSetting();
        }
        public DataSet GetDataSet(string NameSP, List<SqlParameter> ListParam)
        {
            using (SqlConnection con = new(Connection))
            {
                DataSet DataSet = new();
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new(NameSP, con))
                    {
                        cmd.CommandTimeout = 300;
                        cmd.CommandType = CommandType.StoredProcedure;

                        foreach (SqlParameter param in ListParam)
                        {
                            cmd.Parameters.Add(param);
                        }
                        using (SqlDataAdapter da = new(cmd))
                        {
                            da.Fill(DataSet);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Error al obtener DataSet.", ex);
                }
                return DataSet;
            }
        }
        public SqlParameter CreateParam(string parameterName, object value, DbType type)
        {
            SqlParameter parametro = new()
            {
                ParameterName = "@" + parameterName,
                Value = value ?? DBNull.Value,
                DbType = type
            };
            return parametro;
        }
        public DataTable GetDataTable(string NameSP, List<SqlParameter> ListParam)
        {
            return GetDataSet(NameSP, ListParam).Copy().Tables[0];
        }
    }
}
