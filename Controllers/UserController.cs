using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    // ---------------------------
    // CREATE EMPLOYEE
    // ---------------------------
    [Authorize(Roles = "Employee")]
    [HttpPost("create-employee")]
    public IActionResult CreateEmployee(CreateUserDTO dto)
    {
        var exists = _context.Users.Any(u => u.AccountNumber == dto.AccountNumber);
        if (exists)
            return BadRequest("User already exists");

        var hasher = new PasswordHasher<User>();

        var user = new User
        {
            FullName = dto.FullName,
            IdNumber = dto.IdNumber,
            AccountNumber = dto.AccountNumber,
            Role = "Employee",
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = hasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok(new { message = "Employee created successfully" });
    }

    // ---------------------------
    // GET ALL USERS (EMPLOYEE VIEW)
    // ---------------------------
    [Authorize(Roles = "Employee")]
    [HttpGet("all")]
    public IActionResult GetAllUsers()
    {
        var users = _context.Users
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.AccountNumber,
                u.Role,
                u.CreatedAt
            })
            .ToList();

        return Ok(users);
    }

    // ---------------------------
    // GET ONLY CUSTOMERS
    // ---------------------------
    [Authorize(Roles = "Employee")]
    [HttpGet("customers")]
    public IActionResult GetCustomers()
    {
        var customers = _context.Users
            .Where(u => u.Role == "Customer")
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.AccountNumber,
                u.CreatedAt
            })
            .ToList();

        return Ok(customers);
    }

    // ---------------------------
    // DELETE USER
    // ---------------------------
    [Authorize(Roles = "Employee")]
    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);

        if (user == null)
            return NotFound("User not found");

        _context.Users.Remove(user);
        _context.SaveChanges();

        return Ok(new { message = "User deleted successfully" });
    }
}