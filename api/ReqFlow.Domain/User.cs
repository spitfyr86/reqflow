namespace ReqFlow.Domain;

public sealed class User
{
    private User() { }

    public User(Guid id, string email, string displayName, UserRole role, bool isActive = true)
    {
        Id = id;
        Email = Require(email, nameof(email));
        DisplayName = Require(displayName, nameof(displayName));
        Role = role;
        IsActive = isActive;
    }

    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }

    private static string Require(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{name} is required.", name);
        }

        return value.Trim();
    }
}
