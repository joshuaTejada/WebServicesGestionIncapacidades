namespace WebServicesGestionIncapacidades.Models.Class
{
    public class DisabilityClass
    {
        public int Id { get; set; }
        public int id_fondo { get; set; }
        public string descripcionFondo { get; set; }
        public int id_tipo_incapacidad { get; set; }
        public string descripcionIncapacidad { get; set; }
        public string fecha_inicial { get; set; }
        public int dias { get; set; }
        public string numero_incapacidad { get; set; }
        public string incapacidad_inicial { get; set; }
        public string Diagnostico { get; set; }
        public string desc_diagnostico { get; set; }
        public bool es_prorroga { get; set; }
        public bool es_transito { get; set; }
        public bool es_transcrita { get; set; }
    }
}
