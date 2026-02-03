using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetCRM.Modules.MedicalRecords.Migrations
{
    public partial class Init_MedicalRecords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MedicalRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppointmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    PetId = table.Column<Guid>(type: "uuid", nullable: false),
                    VeterinarianUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Complaint = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Diagnosis = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    TreatmentPlan = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Prescription = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Attachments = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vaccinations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MedicalRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    VaccineName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    VaccinationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    NextDueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Batch = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Manufacturer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vaccinations", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Vaccinations");
            migrationBuilder.DropTable(name: "MedicalRecords");
        }
    }
}
