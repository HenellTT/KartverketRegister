namespace KartverketRegister.Models;

public class ErrorViewModel
{
    //ID for forespørselen (kan være null)
    public string? RequestId { get; set; }
    //sjekker at RequestID finnes, returnerer true (ikke tom eller null)
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
