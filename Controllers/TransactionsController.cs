using InternationalPaymentsAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TransactionsController(AppDbContext context)
    {
        _context = context;
    }

    // =======================================
    // EMPLOYEE ONLY: View pending payments
    // =======================================
    [HttpGet("pending")]
    [Authorize(Roles = "Employee")]
    public IActionResult GetPending()
    {
        var payments = _context.Payments
            .Where(p => p.Status == PaymentStatus.Pending)
            .ToList();

        return Ok(payments);
    }

    // =======================================
    // EMPLOYEE ONLY: Verify payment
    // =======================================
    [HttpPut("verify/{id}")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> Verify(int id)
    {
        var payment = await _context.Payments.FindAsync(id);

        if (payment == null)
            return NotFound("Payment not found");

        // BUSINESS RULE: Only pending payments can be verified
        if (payment.Status != PaymentStatus.Pending)
            return BadRequest("Only pending payments can be verified.");

        payment.Status = PaymentStatus.Verified;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Payment verified successfully",
            payment
        });
    }

    // =======================================
    // EMPLOYEE ONLY: Submit to SWIFT
    // =======================================
    [HttpPut("submit/{id}")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> Submit(int id)
    {
        var payment = await _context.Payments.FindAsync(id);

        if (payment == null)
            return NotFound("Payment not found");

        // BUSINESS RULE: Must be verified first
        if (payment.Status != PaymentStatus.Verified)
            return BadRequest("Only verified payments can be submitted to SWIFT.");

        payment.Status = PaymentStatus.Submitted;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Payment submitted to SWIFT successfully",
            payment
        });
    }
}