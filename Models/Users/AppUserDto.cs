namespace KartverketRegister.Models.Users;

/// <summary>
/// Data Transfer Object for brukerinformasjon.
/// Brukes for å eksponere brukerdata uten sensitive felter.
/// </summary>
public class AppUserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Organization { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();
}

