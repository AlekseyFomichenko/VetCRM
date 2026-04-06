-- VetCRM: Init_Pets
-- Таблица pets (питомцы).

CREATE TABLE IF NOT EXISTS "pets" (
    "Id" uuid NOT NULL,
    "ClientId" uuid NULL,
    "Name" character varying(100) NOT NULL,
    "Species" character varying(50) NOT NULL,
    "BirthDate" date NULL,
    "Status" integer NOT NULL,
    CONSTRAINT "PK_pets" PRIMARY KEY ("Id")
);
