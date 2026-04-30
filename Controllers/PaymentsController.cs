using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PaymentsController(AppDbContext context)
    {
        _context = context;
    }

    // =====================================================
    // CUSTOMER: CREATE PAYMENT
    // =====================================================
    [HttpPost("create")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CreatePayment(PaymentDTO dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null)
            return Unauthorized("Invalid token");

        var userId = int.Parse(userIdClaim);

        // VALIDATION
        if (!ValidationHelper.IsValidAmount(dto.Amount))
            return BadRequest("Invalid amount");

        if (!ValidationHelper.IsValidCurrency(dto.Currency))
            return BadRequest("Invalid currency");

        if (!ValidationHelper.IsValidProvider(dto.Provider))
            return BadRequest("Invalid provider");

        if (!ValidationHelper.IsValidAccountNumber(dto.PayeeAccountNumber))
            return BadRequest("Invalid account number");

        if (!ValidationHelper.IsValidSwiftCode(dto.SwiftCode))
            return BadRequest("Invalid SWIFT code");

        // CREATE PAYMENT
        var payment = new Payment
        {
            Amount = dto.Amount,
            Currency = dto.Currency,
            Provider = dto.Provider,
            PayeeAccountNumber = dto.PayeeAccountNumber,
            SwiftCode = dto.SwiftCode,
            Status = "Pending",
            CustomerId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Payment created successfully",
            paymentId = payment.Id
        });
    }

    // =====================================================
    // CUSTOMER: VIEW OWN PAYMENTS
    // =====================================================
    [HttpGet("my-payments")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyPayments()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null)
            return Unauthorized("Invalid token");

        var userId = int.Parse(userIdClaim);

        var payments = await _context.Payments
            .Where(p => p.CustomerId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return Ok(payments);
    }

    // =====================================================
    // EMPLOYEE: VIEW ALL PAYMENTS (FOR TABS UI)
    // =====================================================
    [HttpGet("all")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> GetAllPayments()
    {
        var payments = await _context.Payments
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return Ok(payments);
    }

    // =====================================================
    // EMPLOYEE: VIEW PENDING ONLY
    // =====================================================
    [HttpGet("pending")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> GetPendingPayments()
    {
        var payments = await _context.Payments
            .Where(p => p.Status == "Pending")
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return Ok(payments);
    }

    // =====================================================
    // EMPLOYEE: VIEW VERIFIED ONLY (NEW)
    // =====================================================
    [HttpGet("verified")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> GetVerifiedPayments()
    {
        var payments = await _context.Payments
            .Where(p => p.Status == "Verified")
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return Ok(payments);
    }

    // =====================================================
    // EMPLOYEE: VIEW COMPLETED ONLY (NEW)
    // =====================================================
    [HttpGet("completed")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> GetCompletedPayments()
    {
        var payments = await _context.Payments
            .Where(p => p.Status == "Completed")
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return Ok(payments);
    }

    // =====================================================
    // EMPLOYEE: VERIFY PAYMENT
    // =====================================================
    [HttpPost("verify/{id}")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> VerifyPayment(int id)
    {
        var payment = await _context.Payments.FindAsync(id);

        if (payment == null)
            return NotFound("Payment not found");

        if (payment.Status != "Pending")
            return BadRequest("Only pending payments can be verified");

        var employeeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        payment.Status = "Verified";
        payment.VerifiedByEmployeeId = employeeId;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Payment verified successfully" });
    }

    // =====================================================
    // EMPLOYEE: COMPLETE PAYMENT
    // =====================================================
    [HttpPost("complete/{id}")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> CompletePayment(int id)
    {
        var payment = await _context.Payments.FindAsync(id);

        if (payment == null)
            return NotFound("Payment not found");

        if (payment.Status != "Verified")
            return BadRequest("Only verified payments can be completed");

        payment.Status = "Completed";

        await _context.SaveChangesAsync();

        return Ok(new { message = "Payment sent via SWIFT (simulated)" });
    }
}