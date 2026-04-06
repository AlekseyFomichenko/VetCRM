-- VetCRM: Init Notifications (ReminderLogs)

CREATE TABLE IF NOT EXISTS "ReminderLogs" (
    "Id" uuid NOT NULL,
    "Type" integer NOT NULL,
    "TargetClientId" uuid NULL,
    "TargetPetId" uuid NULL,
    "Channel" integer NOT NULL,
    "Payload" character varying(4000) NOT NULL,
    "Status" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "Error" character varying(1000) NULL,
    CONSTRAINT "PK_ReminderLogs" PRIMARY KEY ("Id")
);
