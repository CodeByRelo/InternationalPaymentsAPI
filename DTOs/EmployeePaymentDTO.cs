public class EmployeePaymentDTO
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; }

    public string Provider { get; set; }

    public string PayeeAccountNumber { get; set; }

    public string SwiftCode { get; set; }

    public string Status { get; set; }

    public DateTime CreatedAt { get; set; }

    // Customer Info
    public string CustomerName { get; set; }

    public string CustomerAccountNumber { get; set; }

    public string CustomerIdNumber { get; set; }
}