public class User
{
    public int Id { get; set; }

    public string FullName { get; set; }

    public string IdNumber { get; set; }

    public string AccountNumber { get; set; } // role-dependent usage

    public string PasswordHash { get; set; }

    public string Role { get; set; } // Customer / Employee

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public List<Payment> Payments { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; }
    public List<LoginAudit> LoginAudits { get; set; }
}