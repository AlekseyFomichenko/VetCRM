namespace VetCRM.Modules.Clients.Domain
{
    public sealed class Client
    {
        public Guid Id { get; private set; }
        public string FullName { get; private set; } = string.Empty;
        public string Phone { get; private set; } = string.Empty;
        public string? Email { get; private set; }
        public string? Address { get; private set; }
        public string? Notes { get; private set; }
        public ClientStatus Status { get; private set; }
        public DateOnly CreatedAt { get; private set; }

        private Client() { }

        private Client(Guid id, string fullName, string phone, string? email, string? address, string? notes, DateOnly createdAt)
        {
            Id = id;
            FullName = fullName;
            Phone = phone;
            Email = email;
            Address = address;
            Notes = notes;
            Status = ClientStatus.Active;
            CreatedAt = createdAt;
        }

        public static Client Create(string fullName, string phone, string? email, string? address, string? notes)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name is required.");
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone is required.");

            return new Client(
                Guid.NewGuid(),
                fullName.Trim(),
                phone.Trim(),
                email?.Trim(),
                address?.Trim(),
                notes?.Trim(),
                DateOnly.FromDateTime(DateTime.UtcNow));
        }

        public void Update(string fullName, string phone, string? email, string? address, string? notes)
        {
            if (Status != ClientStatus.Active)
                throw new InvalidOperationException("Only active clients can be updated.");
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name is required.");
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone is required.");

            FullName = fullName.Trim();
            Phone = phone.Trim();
            Email = email?.Trim();
            Address = address?.Trim();
            Notes = notes?.Trim();
        }

        public void Archive()
        {
            if (Status != ClientStatus.Active)
                throw new InvalidOperationException("Only active clients can be archived.");

            Status = ClientStatus.Archived;
        }
    }
}
