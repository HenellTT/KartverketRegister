namespace KartverketRegister.Models.Responses;

/// <summary>
/// Standard API-respons for alle endepunkter.
/// </summary>
public class GeneralResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public object? Data { get; set; }

    public GeneralResponse(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    public GeneralResponse(bool success, string message, object? data)
    {
        Success = success;
        Message = message;
        Data = data;
    }
}

