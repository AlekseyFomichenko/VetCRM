using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetCRM.SharedKernel
{
    public sealed class ClientNotFoundException : DomainException
    {
        public Guid ClientId { get; }
        public ClientNotFoundException(Guid clientId) : base($"Client with id '{clientId}' was not found.")
        {
            ClientId = clientId;
        }
    }
}
