namespace WebServicesGestionIncapacidades.Models.DataBase.Utilities
{
    public class Utilities
    {
        private readonly IConfiguration _configuration;

        public Utilities(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string RetornarSetting()
        {
            return _configuration.GetConnectionString("ConnectionSQL");
        }
    }
}