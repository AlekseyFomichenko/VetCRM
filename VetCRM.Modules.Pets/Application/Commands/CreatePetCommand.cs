using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetCRM.Modules.Pets.Application.Commands
{
    public record CreatePetCommand(
        Guid ClientId,
        string Name,
        DateOnly BirthDate);
}
