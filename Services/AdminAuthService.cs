namespace MinimalShop.Services;

public class AdminAuthService(IConfiguration configuration)
{
    private bool _isAuthenticated;

    public bool IsAuthenticated => _isAuthenticated;

    public bool TryLogin(string password)
    {
        var expected = configuration["Admin:Password"] ?? "changeme";
        _isAuthenticated = password == expected;
        return _isAuthenticated;
    }

    public void Logout() => _isAuthenticated = false;
}
