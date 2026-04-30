using System.Collections.Concurrent;

public class RateLimitService
{
    private static readonly ConcurrentDictionary<string, int> Attempts = new();

    private const int MAX_ATTEMPTS = 5; // number of allowed attempts before blocking

    public bool IsBlocked(string key)
    {
        return Attempts.ContainsKey(key) && Attempts[key] >= MAX_ATTEMPTS;
    }

    public void AddAttempt(string key)
    {
        if (!Attempts.ContainsKey(key))
            Attempts[key] = 0;

        Attempts[key]++;
    }

    public void Reset(string key)
    {
        Attempts.TryRemove(key, out _);
    }
}