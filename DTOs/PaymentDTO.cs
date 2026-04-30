using System.ComponentModel.DataAnnotations;

public class PaymentDTO
{
    [Required]
    [Range(0.01, 1000000000, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Required]
    [RegularExpression(@"^[A-Z]{3}$",
        ErrorMessage = "Currency must be a valid 3-letter ISO code (e.g., USD, EUR, ZAR)")]
    public string Currency { get; set; }

    [Required]
    [RegularExpression(@"^[A-Za-z\s]{2,50}$",
        ErrorMessage = "Provider must contain only letters and be 2–50 characters long")]
    public string Provider { get; set; }

    [Required]
    [RegularExpression(@"^[A-Z0-9]{6,20}$",
        ErrorMessage = "Payee Account Number must be 6–20 uppercase letters/numbers")]
    public string PayeeAccountNumber { get; set; }

    [Required]
    [RegularExpression(@"^[A-Z0-9]{8,11}$",
        ErrorMessage = "SWIFT Code must be 8 or 11 uppercase characters")]
    public string SwiftCode { get; set; }
}