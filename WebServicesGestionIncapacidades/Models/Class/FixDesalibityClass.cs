namespace WebServicesGestionIncapacidades.Models.Class
{
    public class FixDesalibityClass
    {
        public DisabilityClass disabilityClass { get; set; }
        public List<DocumentClass> documentClass { get; set; }
        public List<FixDocumentClass> fixDocumentClass { get; set; }
        public companyParameterClass companyparameter { get; set; }
        public string logo { get; set; }
        public string Base64ImgEps { get; set; }
    }
}
