-- VetCRM: Init MedicalRecords and Vaccinations

CREATE TABLE IF NOT EXISTS "MedicalRecords" (
    "Id" uuid NOT NULL,
    "AppointmentId" uuid NOT NULL,
    "PetId" uuid NOT NULL,
    "VeterinarianUserId" uuid NULL,
    "Complaint" character varying(2000) NOT NULL,
    "Diagnosis" character varying(2000) NOT NULL,
    "TreatmentPlan" character varying(2000) NOT NULL,
    "Prescription" character varying(2000) NOT NULL,
    "Attachments" character varying(4000) NULL,
    "CreatedAt" date NOT NULL,
    CONSTRAINT "PK_MedicalRecords" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS "Vaccinations" (
    "Id" uuid NOT NULL,
    "MedicalRecordId" uuid NOT NULL,
    "VaccineName" character varying(200) NOT NULL,
    "VaccinationDate" date NOT NULL,
    "NextDueDate" date NULL,
    "Batch" character varying(100) NULL,
    "Manufacturer" character varying(200) NULL,
    CONSTRAINT "PK_Vaccinations" PRIMARY KEY ("Id")
);
