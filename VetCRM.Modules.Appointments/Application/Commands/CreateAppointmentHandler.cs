using VetCRM.Modules.Appointments.Application.Contracts;
using VetCRM.Modules.Appointments.Domain;
using VetCRM.Modules.Clients.Application.Contracts;
using VetCRM.Modules.Pets.Application.Contracts;
using VetCRM.SharedKernel;

namespace VetCRM.Modules.Appointments.Application.Commands
{
    public sealed class CreateAppointmentHandler(
        IAppointmentRepository repository,
        IPetReadService petReadService,
        IClientReadService clientReadService)
    {
        private readonly IAppointmentRepository _repository = repository;
        private readonly IPetReadService _petReadService = petReadService;
        private readonly IClientReadService _clientReadService = clientReadService;

        public async Task<CreateAppointmentResult> Handle(CreateAppointmentCommand command, CancellationToken ct)
        {
            bool petExists = await _petReadService.ExistsAsync(command.PetId, ct);
            if (!petExists)
                throw new PetNotFoundException(command.PetId);

            bool clientExists = await _clientReadService.ExistsAsync(command.ClientId, ct);
            if (!clientExists)
                throw new ClientNotFoundException(command.ClientId);

            bool hasOverlap = await _repository.HasOverlappingForVetAsync(
                command.VeterinarianUserId,
                command.StartsAt,
                command.EndsAt,
                excludeAppointmentId: null,
                ct);
            if (hasOverlap)
                throw new AppointmentConflictException(command.VeterinarianUserId, command.StartsAt, command.EndsAt);

            Appointment appointment = Appointment.Create(
                command.PetId,
                command.ClientId,
                command.VeterinarianUserId,
                command.StartsAt,
                command.EndsAt,
                command.Reason,
                createdByUserId: null);

            await _repository.AddAsync(appointment, ct);
            return new CreateAppointmentResult(appointment.Id);
        }
    }
}
