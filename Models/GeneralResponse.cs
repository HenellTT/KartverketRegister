namespace KartverketRegister.Models
{
    public class GeneralResponse
    {
        //Generell responsmodell for API-svar
        public bool Success { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }

        public GeneralResponse(bool s, string m) {
            Success = s;
            Message = m;
        }
        public GeneralResponse(bool success, string message, object? data)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }
}
