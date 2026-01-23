namespace WebServicesGestionIncapacidades.Models.Class.Response
{
    public class ResponseModels
    {
        public string? CodeResponse { get; set; } = string.Empty;
        public string? MessageResponse { get; set; } = string.Empty;
        public string? Token { get; set; }
        public DataGeneralClass? Data { get; set; }
    }

    public class CompanyDataResponse
    {
        public string? CodeResponse { get; set; } = string.Empty;
        public string? MessageResponse { get; set; } = string.Empty;
        public string? Token { get; set; }
        public DataCompanyClass? Data { get; set; }
    }
    public class AttachedResponse
    {
        public string? CodeResponse { get; set; } = string.Empty;
        public string? MessageResponse { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? Base64ImgEps { get; set; }
        public List<AttachedRequiredClass>? Data { get; set; }
    }
    public class DiagnosisResponse
    {
        public string? CodeResponse { get; set; } = string.Empty;
        public string? MessageResponse { get; set; } = string.Empty;
        public string? Token { get; set; }
        public List<DiagnosisClass>? Data { get; set; }
    }
    public class DisabilityResponse
    {
        public string? CodeResponse { get; set; } = string.Empty;
        public string? MessageResponse { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? Data { get; set; }
    }
    public class FixDesabilityResponse
    {
        public string? CodeResponse { get; set; } = string.Empty;
        public string? MessageResponse { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? identificacion { get; set; }

        public FixDesalibityClass? Data { get; set; }
    }
    public class healthFundResponse
    {
        public string? CodeResponse { get; set; } = string.Empty;
        public string? MessageResponse { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? IdFund { get; set; }
    }
    public class DisabilityListResponse
    {
        public string? CodeResponse { get; set; } = string.Empty;
        public string? MessageResponse { get; set; } = string.Empty;
        public string? Token { get; set; }
        public List<GeneralDisabilityClass>? Data { get; set; }
    }
}
