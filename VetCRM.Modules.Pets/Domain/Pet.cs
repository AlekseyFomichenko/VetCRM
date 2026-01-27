using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetCRM.Modules.Pets.Domain
{
    public sealed class Pet
    {
        public Guid Id { get; private set; }
        public Guid? ClientId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Species { get; private set; } = string.Empty;
        public DateOnly BirthDate { get; private set; }
        public PetStatus Status { get; private set; }

        private Pet() { }

        private Pet (Guid id, Guid? clientId, string name, string species, DateOnly birthDate)
        {
            Id = id;
            ClientId = clientId;
            Name = name;
            Species = species;
            BirthDate = birthDate;
            Status = PetStatus.Active;
        }

        public static Pet Create(Guid? clientId, string name, string species, DateOnly birthDate)
        {
            if (clientId == Guid.Empty) throw new ArgumentException("ClientId is required");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Pet name is required");
            
            return new Pet(
                Guid.NewGuid(),
                clientId,
                name.Trim(),
                species,
                birthDate);
        }

        public void Archive()
        {
            if (Status != PetStatus.Active) throw new InvalidOperationException("Only active pets can be archived");

            Status = PetStatus.Archived;
        }

        public void ChangeClient(Guid clientId)
        {
            ClientId = clientId;
        }

        public void UnassignFromClient()
        {
            ClientId = null;
        }
    }
}
