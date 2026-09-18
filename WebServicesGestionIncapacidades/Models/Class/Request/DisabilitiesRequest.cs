namespace WebServicesGestionIncapacidades.Models.Class.Request
{
    public class DisabilitiesRequest
    {
        public string? ID { get; set; }
        public bool? Prescriptions { get; set; }
        public bool? Extension { get; set; }
        public bool? Transit { get; set; }
        public string? InitialDisability { get; set; }
        public int? IdTypeDisability { get; set; }
        public string? companyHealth { get; set; }
        public string? NumberDisability { get; set; }
        public string? Diagnosis { get; set; }
        public string? DescriptionDiagnosis { get; set; }
        public string? InitialDate { get; set; }
        public int? Day { get; set; }
        public string Base64Attached1 { get; set; }
        public string? IdAttached1 { get; set; }
        public string? Base64Attached2 { get; set; }
        public string? IdAttached2 { get; set; }
        public string? Base64Attached3 { get; set; }
        public string? IdAttached3 { get; set; }
        public string? Base64Attached4 { get; set; }
        public string? IdAttached4 { get; set; }
        public string? Base64Attached5 { get; set; }
        public string? IdAttached5 { get; set; }
        public string? Base64Attached6 { get; set; }
        public string? IdAttached6 { get; set; }
    }
}