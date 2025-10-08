namespace KartverketRegister.Models
{
    public class GeneralResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public GeneralResponse(bool s, string m) {
            Success = s;
            Message = m;
        }
    }
}
