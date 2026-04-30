using System.Text.RegularExpressions;

public static class ValidationHelper
{
    // Only numbers with optional decimals
    public static bool IsValidAmount(decimal amount)
    {
        return amount > 0 && amount <= 1000000;
    }

    // Currency whitelist (strict control)
    public static bool IsValidCurrency(string currency)
    {
        return Regex.IsMatch(currency, @"^(ZAR|USD|EUR|GBP)$");
    }

    // SWIFT code format: 8 or 11 uppercase letters/numbers
    public static bool IsValidSwiftCode(string code)
    {
        return Regex.IsMatch(code, @"^[A-Z0-9]{8,11}$");
    }

    // Account number: digits only (8–20 chars)
    public static bool IsValidAccountNumber(string acc)
    {
        return Regex.IsMatch(acc, @"^[0-9]{8,20}$");
    }

    // Provider whitelist (only SWIFT allowed in system)
    public static bool IsValidProvider(string provider)
    {
        return provider == "SWIFT";
    }
}