namespace WebServicesGestionIncapacidades.Models.Class.Request
{
    public class DocumentRequest
    {
        public int IdDisability { get; set; }
        public IFormFile? Base64Attached1 { get; set; }
        public string? IdAttached1 { get; set; }
        public IFormFile? Base64Attached2 { get; set; }
        public string? IdAttached2 { get; set; }
        public IFormFile? Base64Attached3 { get; set; }
        public string? IdAttached3 { get; set; }
        public IFormFile? Base64Attached4 { get; set; }
        public string? IdAttached4 { get; set; }
        public IFormFile? Base64Attached5 { get; set; }
        public string? IdAttached5 { get; set; }
        public IFormFile? Base64Attached6 { get; set; }
        public string? IdAttached6 { get; set; }
    }
}