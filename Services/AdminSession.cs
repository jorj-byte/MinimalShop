namespace MinimalShop.Services;

public class AdminSettings
{
    public const string SectionName = "Admin";
    public string Password { get; set; } = "changeme";
}

public class AdminSession
{
    public bool IsAuthenticated { get; private set; }

    public bool Login(string password, string expectedPassword) =>
        IsAuthenticated = password == expectedPassword;

    public void Logout() => IsAuthenticated = false;
}
