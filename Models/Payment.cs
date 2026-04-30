public class Payment
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; }

    public string Provider { get; set; }

    public string PayeeAccountNumber { get; set; }

    public string SwiftCode { get; set; }

    public string Status { get; set; } // Pending, Verified, Submitted

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // FK
    public int CustomerId { get; set; }
    public User Customer { get; set; }

    public int? VerifiedByEmployeeId { get; set; }
    public User VerifiedByEmployee { get; set; }
}