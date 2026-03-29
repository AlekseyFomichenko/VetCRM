using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using VetCRM.Api.Controllers.MedicalRecords;
using VetCRM.Modules.MedicalRecords.Application.Commands;
using VetCRM.Modules.MedicalRecords.Application.Queries;
using VetCRM.SharedKernel;

namespace VetCRM.Api.Controllers
{
    [ApiController]
    [Route("api/medical-records")]
    [Authorize(Roles = "Admin,Receptionist,Veterinarian")]
    public sealed class MedicalRecordsController(
        CreateMedicalRecordHandler createHandler,
        GetMedicalRecordByIdHandler getByIdHandler,
        GetMedicalRecordsByPetIdHandler getByPetIdHandler,
        UpdateMedicalRecordHandler updateHandler,
        AddVaccinationHandler addVaccinationHandler) : Controller
    {
        private readonly CreateMedicalRecordHandler _createHandler = createHandler;
        private readonly GetMedicalRecordByIdHandler _getByIdHandler = getByIdHandler;
        private readonly GetMedicalRecordsByPetIdHandler _getByPetIdHandler = getByPetIdHandler;
        private readonly UpdateMedicalRecordHandler _updateHandler = updateHandler;
        private readonly AddVaccinationHandler _addVaccinationHandler = addVaccinationHandler;

        [HttpGet("~/api/pets/{petId:guid}/medical-records", Name = "GetMedicalRecordsByPetId")]
        public async Task<IActionResult> GetByPetId(Guid petId, CancellationToken ct)
        {
            var query = new GetMedicalRecordsByPetIdQuery(petId);
            var results = await _getByPetIdHandler.Handle(query, ct);
            var response = results.Select(Map).ToList();
            return Ok(response);
        }

        [HttpPost("~/api/pets/{petId:guid}/medical-records")]
        [Authorize(Roles = "Admin,Veterinarian")]
        public async Task<IActionResult> CreateForPet(Guid petId, [FromBody] CreateMedicalRecordRequest request, CancellationToken ct)
        {
            Guid? veterinarianUserId = null;
            var userIdRaw = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (Guid.TryParse(userIdRaw, out var parsedUserId))
                veterinarianUserId = parsedUserId;

            var command = new CreateMedicalRecordCommand(
                petId,
                veterinarianUserId,
                request.Complaint,
                request.Diagnosis,
                request.TreatmentPlan,
                request.Prescription,
                request.Attachments);
            var medicalRecordId = await _createHandler.Handle(command, ct);
            return Created($"/api/medical-records/{medicalRecordId}", new { medicalRecordId });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _getByIdHandler.Handle(new GetMedicalRecordByIdQuery(id), ct);
            if (result is null)
                throw new MedicalRecordNotFoundException(id);
            return Ok(Map(result));
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,Veterinarian")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMedicalRecordRequest request, CancellationToken ct)
        {
            await _updateHandler.Handle(new UpdateMedicalRecordCommand(
                id,
                request.Complaint,
                request.Diagnosis,
                request.TreatmentPlan,
                request.Prescription,
                request.Attachments), ct);
            return Ok();
        }

        [HttpPost("{id:guid}/vaccinations")]
        [Authorize(Roles = "Admin,Veterinarian")]
        public async Task<IActionResult> AddVaccination(Guid id, [FromBody] AddVaccinationRequest request, CancellationToken ct)
        {
            await _addVaccinationHandler.Handle(new AddVaccinationCommand(
                id,
                request.VaccineName,
                request.VaccinationDate,
                request.NextDueDate,
                request.Batch,
                request.Manufacturer), ct);
            return Ok();
        }

        private static MedicalRecordResponse Map(GetMedicalRecordByIdResult r) =>
            new(r.Id,
                r.AppointmentId,
                r.PetId,
                r.VeterinarianUserId,
                r.Complaint,
                r.Diagnosis,
                r.TreatmentPlan,
                r.Prescription,
                r.Attachments,
                r.CreatedAt,
                r.Vaccinations.Select(v => new VaccinationResponse(v.Id, v.MedicalRecordId, v.VaccineName, v.VaccinationDate, v.NextDueDate, v.Batch, v.Manufacturer)).ToList());
    }
}
