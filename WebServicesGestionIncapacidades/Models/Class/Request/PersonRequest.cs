namespace WebServicesGestionIncapacidades.Models.Class.Request
{
    public class PersonRequest
    {
        public string TypeId { get; set; }
        public string ID { get; set; }
        public string Email { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Cel { get; set; }
        public string? DateStart { get; set; }
        public string? EPS { get; set; }
        public string? NitCompany { get; set; }
        public string? Nomina { get; set; }
    }
}
