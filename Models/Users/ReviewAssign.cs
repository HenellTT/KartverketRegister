namespace KartverketRegister.Models.Users;

/// <summary>
/// Representerer tilordning av en markør til en ansatt for gjennomgang.
/// </summary>
public class ReviewAssign
{
    public int UserId { get; set; }
    public int MarkerId { get; set; }

    public ReviewAssign() { }

    public ReviewAssign(int userId, int markerId)
    {
        UserId = userId;
        MarkerId = markerId;
    }
}

