using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RateLimitService _rateLimit;
    private readonly AuthService _auth;
    private readonly AppDbContext _context;

    public AuthController(AuthService auth, RateLimitService rateLimit, AppDbContext context)
    {
        _auth = auth;
        _rateLimit = rateLimit;
        _context = context;
    }

    // ---------------------------
    // PASSWORD VALIDATION METHOD
    // ---------------------------
    private string ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return "Password is required";

        if (password.Length < 8)
            return "Password must be at least 8 characters long";

        if (!password.Any(char.IsUpper))
            return "Password must contain at least one uppercase letter";

        if (!password.Any(char.IsLower))
            return "Password must contain at least one lowercase letter";

        if (!password.Any(char.IsDigit))
            return "Password must contain at least one number";

        return null; // valid
    }

    // ---------------------------
    // REGISTER (CUSTOMER)
    // ---------------------------
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO dto)
    {
        // 🔐 Password validation (already added earlier)
        var passwordError = ValidatePassword(dto.Password);
        if (passwordError != null)
            return BadRequest(passwordError);

        // Create user
        var user = await _auth.Register(dto);

        // 🔥 AUTO LOGIN: generate token immediately making sure that the user is auto-logged in after registration
        var token = _auth.Login(new LoginDTO
        {
            AccountNumber = dto.AccountNumber,
            Password = dto.Password
        });

        return Ok(new
        {
            message = "Registration successful",
            token
        });
    }

    // ---------------------------
    // LOGIN
    // ---------------------------
    [HttpPost("login")]
    public IActionResult Login(LoginDTO dto)
    {
        string key = dto.AccountNumber;

        if (_rateLimit.IsBlocked(key))
            return BadRequest("Too many failed attempts. Try again later.");

        var token = _auth.Login(dto);

        if (token == null)
        {
            _rateLimit.AddAttempt(key);
            return Unauthorized("Invalid credentials");
        }

        _rateLimit.Reset(key);

        return Ok(new { token });
    }

    // ---------------------------
    // BOOTSTRAP ADMIN
    // ---------------------------
    [HttpPost("bootstrap-admin")]
    [AllowAnonymous] // TEMP - REMOVE LATER
    public IActionResult BootstrapAdmin()
    {
        try
        {
            // ---------------------------
            // CHECK IF EMPLOYEE EXISTS
            // ---------------------------
            var existing = _context.Users
                .FirstOrDefault(u => u.AccountNumber == "EMP001");

            if (existing != null)
            {
                return Ok(new
                {
                    message = "Employee already exists",
                    accountNumber = existing.AccountNumber,
                    role = existing.Role,
                    createdAt = existing.CreatedAt
                });
            }

            // ---------------------------
            // CREATE EMPLOYEE
            // ---------------------------
            var hasher = new PasswordHasher<User>();

            var employee = new User
            {
                FullName = "System Employee",
                IdNumber = "0000000000000",
                AccountNumber = "EMP001",
                Role = "Employee",
                CreatedAt = DateTime.UtcNow
            };

            // 🔐 NOTE: Even bootstrap follows hashing
            employee.PasswordHash = hasher.HashPassword(employee, "ADMIN123");

            _context.Users.Add(employee);
            _context.SaveChanges();

            return Ok(new
            {
                message = "Employee created successfully",
                accountNumber = employee.AccountNumber,
                password = "ADMIN123",
                role = employee.Role
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Bootstrap failed",
                error = ex.Message
            });
        }
    }
}