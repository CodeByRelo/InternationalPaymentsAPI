using System.ComponentModel.DataAnnotations;

public class RegisterDTO
{
    [Required]
    [RegularExpression(@"^[A-Za-z\s]{3,50}$",
        ErrorMessage = "Full name must contain only letters and be 3–50 characters long")]
    public string FullName { get; set; }

    [Required]
    [RegularExpression(@"^\d{13}$",
        ErrorMessage = "ID Number must be exactly 13 digits")]
    public string IdNumber { get; set; }

    [Required]
    [RegularExpression(@"^[A-Z0-9]{5,10}$",
        ErrorMessage = "Account Number must be 5–10 uppercase letters/numbers only")]
    public string AccountNumber { get; set; }

    [Required]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    public string Password { get; set; }
}