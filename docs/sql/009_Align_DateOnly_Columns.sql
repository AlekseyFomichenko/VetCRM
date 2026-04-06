-- VetCRM: приведение колонок к типу date (как в миграциях EF + Npgsql DateOnly).
-- Нужно, если таблицы созданы старыми 002/003/005 с timestamp with time zone.

DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM information_schema.columns c
        WHERE c.table_schema = 'public'
          AND c.table_name = 'Clients'
          AND c.column_name = 'CreatedAt'
          AND c.data_type = 'timestamp with time zone'
    ) THEN
        ALTER TABLE "Clients" ALTER COLUMN "CreatedAt" TYPE date USING ("CreatedAt"::date);
    END IF;

    IF EXISTS (
        SELECT 1
        FROM information_schema.columns c
        WHERE c.table_schema = 'public'
          AND c.table_name = 'pets'
          AND c.column_name = 'BirthDate'
          AND c.data_type = 'timestamp with time zone'
    ) THEN
        ALTER TABLE pets ALTER COLUMN "BirthDate" TYPE date USING ("BirthDate"::date);
    END IF;

    IF EXISTS (
        SELECT 1
        FROM information_schema.columns c
        WHERE c.table_schema = 'public'
          AND c.table_name = 'MedicalRecords'
          AND c.column_name = 'CreatedAt'
          AND c.data_type = 'timestamp with time zone'
    ) THEN
        ALTER TABLE "MedicalRecords" ALTER COLUMN "CreatedAt" TYPE date USING ("CreatedAt"::date);
    END IF;

    IF EXISTS (
        SELECT 1
        FROM information_schema.columns c
        WHERE c.table_schema = 'public'
          AND c.table_name = 'Vaccinations'
          AND c.column_name = 'VaccinationDate'
          AND c.data_type = 'timestamp with time zone'
    ) THEN
        ALTER TABLE "Vaccinations" ALTER COLUMN "VaccinationDate" TYPE date USING ("VaccinationDate"::date);
    END IF;

    IF EXISTS (
        SELECT 1
        FROM information_schema.columns c
        WHERE c.table_schema = 'public'
          AND c.table_name = 'Vaccinations'
          AND c.column_name = 'NextDueDate'
          AND c.data_type = 'timestamp with time zone'
    ) THEN
        ALTER TABLE "Vaccinations" ALTER COLUMN "NextDueDate" TYPE date USING ("NextDueDate"::date);
    END IF;
END $$;
