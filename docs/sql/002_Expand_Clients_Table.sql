-- VetCRM: Expand_Clients_Table
-- Добавление колонок в таблицу Clients.
-- Выполнять после 001_Init_Clients.

ALTER TABLE "Clients"
    ADD COLUMN IF NOT EXISTS "Address" character varying(500) NULL,
    ADD COLUMN IF NOT EXISTS "CreatedAt" date NOT NULL DEFAULT DATE '0001-01-01',
    ADD COLUMN IF NOT EXISTS "Email" character varying(200) NULL,
    ADD COLUMN IF NOT EXISTS "FullName" character varying(200) NOT NULL DEFAULT '',
    ADD COLUMN IF NOT EXISTS "Notes" character varying(2000) NULL,
    ADD COLUMN IF NOT EXISTS "Phone" character varying(50) NOT NULL DEFAULT '',
    ADD COLUMN IF NOT EXISTS "Status" integer NOT NULL DEFAULT 1;

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Clients_Phone" ON "Clients" ("Phone");
