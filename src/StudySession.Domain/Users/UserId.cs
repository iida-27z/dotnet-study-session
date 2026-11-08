namespace StudySession.Domain.Users;

public record UserId
{
    public Guid Value { get; }

    public static UserId New()
    {
        return From(Guid.NewGuid());
    }

    public static UserId From(Guid value)
    {
        return new UserId(value);
    }

    public static implicit operator Guid(UserId userId)
    {
        ArgumentNullException.ThrowIfNull(userId);

        return userId.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    private UserId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("User ID must not be empty.", nameof(value));
        }

        Value = value;
    }
}
