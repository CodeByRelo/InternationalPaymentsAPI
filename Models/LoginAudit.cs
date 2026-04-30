public class LoginAudit
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string AccountNumber { get; set; }

    public bool IsSuccessful { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public string IpAddress { get; set; }
}