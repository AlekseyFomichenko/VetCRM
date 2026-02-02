-- VetCRM: Init_Clients
-- Таблица Clients (минимальная: только Id).
-- Выполнять первой, если база пустая.

CREATE TABLE IF NOT EXISTS "Clients" (
    "Id" uuid NOT NULL,
    CONSTRAINT "PK_Clients" PRIMARY KEY ("Id")
);
