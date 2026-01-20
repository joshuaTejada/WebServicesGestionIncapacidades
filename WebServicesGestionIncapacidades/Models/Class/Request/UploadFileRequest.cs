namespace WebServicesGestionIncapacidades.Models.Class.Request
{
    public class UploadFileRequest
    {
        public string? SessionId { get; set; }
        public IFormFile? File { get; set; }
        public int? Order { get; set; }
        public string? Identificacion { get; set; }
    }
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

}
