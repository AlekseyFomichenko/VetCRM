-- VetCRM: Init Appointments

CREATE TABLE IF NOT EXISTS "Appointments" (
    "Id" uuid NOT NULL,
    "PetId" uuid NOT NULL,
    "ClientId" uuid NOT NULL,
    "VeterinarianUserId" uuid NULL,
    "StartsAt" timestamp with time zone NOT NULL,
    "EndsAt" timestamp with time zone NOT NULL,
    "Status" integer NOT NULL,
    "Reason" character varying(500) NULL,
    "CreatedByUserId" uuid NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Appointments" PRIMARY KEY ("Id")
);

CREATE INDEX IF NOT EXISTS "IX_Appointments_VeterinarianUserId_StartsAt"
    ON "Appointments" ("VeterinarianUserId", "StartsAt");
