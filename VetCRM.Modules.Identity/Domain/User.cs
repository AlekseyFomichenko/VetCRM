namespace VetCRM.Modules.Identity.Domain
{
    public sealed class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public UserRole Role { get; private set; }
        public string? FullName { get; private set; }
        public UserStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private User() { }

        private User(Guid id, string email, string passwordHash, UserRole role, string? fullName, DateTime createdAt)
        {
            Id = id;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            FullName = fullName;
            Status = UserStatus.Active;
            CreatedAt = createdAt;
        }

        public static User Create(string email, string passwordHash, UserRole role, string? fullName = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash is required.");

            return new User(
                Guid.NewGuid(),
                email.Trim().ToLowerInvariant(),
                passwordHash,
                role,
                fullName?.Trim(),
                DateTime.UtcNow);
        }

        public void SetPassword(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash is required.");
            PasswordHash = passwordHash;
        }

        public void Update(string? fullName, UserRole? role)
        {
            if (Status != UserStatus.Active)
                throw new InvalidOperationException("Only active users can be updated.");
            if (fullName is not null)
                FullName = fullName.Trim();
            if (role.HasValue)
                Role = role.Value;
        }

        public void Disable()
        {
            if (Status != UserStatus.Active)
                throw new InvalidOperationException("Only active users can be disabled.");
            Status = UserStatus.Disabled;
        }
    }
}
