public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // ---------------------------
        // CLICKJACKING PROTECTION
        // ---------------------------
        context.Response.Headers["X-Frame-Options"] = "DENY";

        // ---------------------------
        // MIME-TYPE SNIFFING PROTECTION
        // ---------------------------
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";

        // ---------------------------
        // XSS PROTECTION
        // ---------------------------
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";

        // ---------------------------
        // CONTENT SECURITY POLICY (CSP)
        // ---------------------------
        context.Response.Headers["Content-Security-Policy"] =
            "default-src 'self'; " +
            "img-src 'self' data:; " +
            "script-src 'self'; " +
            "style-src 'self' 'unsafe-inline';";

        // ---------------------------
        // HSTS (FOR HTTPS ENFORCEMENT)
        // ---------------------------
        context.Response.Headers["Strict-Transport-Security"] =
            "max-age=31536000; includeSubDomains";

        await _next(context);
    }
}