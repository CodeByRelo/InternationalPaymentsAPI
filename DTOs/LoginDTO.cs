using System.ComponentModel.DataAnnotations;

public class LoginDTO
{
    [Required]
    [RegularExpression(@"^[A-Z0-9]{5,10}$",
        ErrorMessage = "Invalid account number format")]
    public string AccountNumber { get; set; }

    [Required]
    public string Password { get; set; }
}