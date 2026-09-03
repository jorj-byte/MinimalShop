using System.Collections.Concurrent;
using Microsoft.AspNetCore.Identity;

namespace MinimalShop.Services;

public sealed class AdminAuthOptions
{
    public const string SectionName = "Admin";
    public string Password { get; set; } = string.Empty;
    public int MaxFailedAttempts { get; set; } = 5;
    public int LockoutMinutes { get; set; } = 15;
}

public sealed class AdminAuthService
{
    private readonly IPasswordHasher<object> _passwordHasher = new PasswordHasher<object>();
    private readonly string _passwordHash;
    private readonly int _maxFailedAttempts;
    private readonly TimeSpan _lockoutDuration;
    private readonly ConcurrentDictionary<string, AttemptState> _attempts = new(StringComparer.Ordinal);
    private readonly ILogger<AdminAuthService> _logger;

    public AdminAuthService(IConfiguration configuration, ILogger<AdminAuthService> logger)
    {
        _logger = logger;
        var options = configuration.GetSection(AdminAuthOptions.SectionName).Get<AdminAuthOptions>()
                      ?? new AdminAuthOptions();

        _maxFailedAttempts = Math.Max(3, options.MaxFailedAttempts);
        _lockoutDuration = TimeSpan.FromMinutes(Math.Max(5, options.LockoutMinutes));

        var password = options.Password;
        if (string.IsNullOrWhiteSpace(password) || password == "changeme")
        {
            _logger.LogWarning(
                "Admin password is missing or still the default 'changeme'. Set Admin__Password to a strong value.");
            password = string.IsNullOrWhiteSpace(password) ? "changeme" : password;
        }

        // Hash once at startup so VerifyHashedPassword can be used on each attempt.
        _passwordHash = _passwordHasher.HashPassword(new object(), password);
    }

    public AuthResult Validate(string? password, string? clientKey)
    {
        var key = string.IsNullOrWhiteSpace(clientKey) ? "unknown" : clientKey.Trim();
        var state = _attempts.GetOrAdd(key, _ => new AttemptState());

        lock (state)
        {
            if (state.LockoutUntil is { } until && until > DateTimeOffset.UtcNow)
            {
                var seconds = (int)Math.Ceiling((until - DateTimeOffset.UtcNow).TotalSeconds);
                return AuthResult.Fail($"Too many failed attempts. Try again in {seconds} seconds.");
            }

            if (state.LockoutUntil is not null && state.LockoutUntil <= DateTimeOffset.UtcNow)
            {
                state.FailedCount = 0;
                state.LockoutUntil = null;
            }

            var provided = password ?? string.Empty;
            var verify = _passwordHasher.VerifyHashedPassword(new object(), _passwordHash, provided);
            if (verify is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded)
            {
                state.FailedCount = 0;
                state.LockoutUntil = null;
                return AuthResult.Ok();
            }

            // Constant-time-ish dummy work already done by PasswordHasher; still avoid leaking timing via logs.
            state.FailedCount++;
            if (state.FailedCount >= _maxFailedAttempts)
            {
                state.LockoutUntil = DateTimeOffset.UtcNow.Add(_lockoutDuration);
                state.FailedCount = 0;
                _logger.LogWarning("Admin login locked out for key {ClientKey} for {Minutes} minutes", key, _lockoutDuration.TotalMinutes);
                return AuthResult.Fail($"Too many failed attempts. Locked for {(int)_lockoutDuration.TotalMinutes} minutes.");
            }

            var remaining = _maxFailedAttempts - state.FailedCount;
            return AuthResult.Fail($"Invalid password. {remaining} attempt(s) remaining.");
        }
    }

    private sealed class AttemptState
    {
        public int FailedCount { get; set; }
        public DateTimeOffset? LockoutUntil { get; set; }
    }
}

public readonly record struct AuthResult(bool Succeeded, string? Error)
{
    public static AuthResult Ok() => new(true, null);
    public static AuthResult Fail(string error) => new(false, error);
}
