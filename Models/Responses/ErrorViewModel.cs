namespace KartverketRegister.Models.Responses;

/// <summary>
/// ViewModel for feilvisning.
/// </summary>
public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

