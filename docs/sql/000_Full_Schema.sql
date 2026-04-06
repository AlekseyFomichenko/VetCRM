-- VetCRM: полная схема с нуля (одним запуском)
-- Для полностью пустой базы. Если "Clients" уже есть только с Id — сначала выполните 002_Expand_Clients_Table.sql.

-- Clients (полная схема)
CREATE TABLE IF NOT EXISTS "Clients" (
    "Id" uuid NOT NULL,
    "FullName" character varying(200) NOT NULL,
    "Phone" character varying(50) NOT NULL,
    "Email" character varying(200) NULL,
    "Address" character varying(500) NULL,
    "Notes" character varying(2000) NULL,
    "Status" integer NOT NULL,
    "CreatedAt" date NOT NULL,
    CONSTRAINT "PK_Clients" PRIMARY KEY ("Id")
);

-- Если таблица Clients уже существовала с одной колонкой Id — добавляем остальные колонки
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public' AND table_name = 'Clients' AND column_name = 'Phone'
    ) THEN
        ALTER TABLE "Clients"
            ADD COLUMN "FullName" character varying(200) NOT NULL DEFAULT '',
            ADD COLUMN "Phone" character varying(50) NOT NULL DEFAULT '',
            ADD COLUMN "Email" character varying(200) NULL,
            ADD COLUMN "Address" character varying(500) NULL,
            ADD COLUMN "Notes" character varying(2000) NULL,
            ADD COLUMN "Status" integer NOT NULL DEFAULT 1,
            ADD COLUMN "CreatedAt" date NOT NULL DEFAULT DATE '0001-01-01';
    END IF;
END $$;

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Clients_Phone" ON "Clients" ("Phone");

-- Pets
CREATE TABLE IF NOT EXISTS "pets" (
    "Id" uuid NOT NULL,
    "ClientId" uuid NULL,
    "Name" character varying(100) NOT NULL,
    "Species" character varying(50) NOT NULL,
    "BirthDate" date NULL,
    "Status" integer NOT NULL,
    CONSTRAINT "PK_pets" PRIMARY KEY ("Id")
);
