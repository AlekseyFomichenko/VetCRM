using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetCRM.Modules.Pets.Application.Commands
{
    public sealed record CreatePetCommand(
        Guid? ClientId,
        string Name,
        string Species,
        DateOnly? BirthDate);
}
