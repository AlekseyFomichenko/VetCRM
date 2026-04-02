using VetCRM.Modules.Clients.Application.Contracts;
using VetCRM.Modules.MedicalRecords.Application.Contracts;
using VetCRM.Modules.Pets.Application.Contracts;

namespace VetCRM.Modules.Reports.Application.Queries
{
    public sealed class GetOverdueVaccinationsReportHandler(
        IUpcomingVaccinationsQuery upcomingVaccinationsQuery,
        IPetReadService petReadService,
        IClientReadService clientReadService)
    {
        private const int SoonDays = 7;

        private readonly IUpcomingVaccinationsQuery _upcomingVaccinationsQuery = upcomingVaccinationsQuery;
        private readonly IPetReadService _petReadService = petReadService;
        private readonly IClientReadService _clientReadService = clientReadService;

        public async Task<GetOverdueVaccinationsReportResult> Handle(
            GetOverdueVaccinationsReportQuery query,
            CancellationToken ct)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var upToExclusive = today.AddDays(SoonDays + 1);

            var upcoming = await _upcomingVaccinationsQuery.GetUpcomingAsync(
                DateOnly.MinValue,
                upToExclusive,
                ct);

            var rows = new List<OverdueVaccinationReportDto>();

            foreach (var v in upcoming)
            {
                Guid clientId = _petReadService.GetOwnerClientId(v.PetId);
                string? clientFullName = null;
                string? clientPhone = null;
                string? clientEmail = null;

                if (clientId != Guid.Empty)
                {
                    var contact = await _clientReadService.GetContactAsync(clientId, ct);
                    if (contact is not null)
                    {
                        clientFullName = contact.FullName;
                        clientPhone = contact.Phone;
                        clientEmail = contact.Email;
                    }
                }

                bool isOverdue = v.NextDueDate < today;

                rows.Add(new OverdueVaccinationReportDto(
                    v.VaccinationId,
                    v.PetId,
                    v.VaccineName,
                    v.NextDueDate,
                    isOverdue,
                    clientFullName,
                    clientPhone,
                    clientEmail));
            }

            int totalCount = rows.Count;
            var paged = rows
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            return new GetOverdueVaccinationsReportResult(totalCount, paged, query.Page, query.PageSize);
        }
    }
}
