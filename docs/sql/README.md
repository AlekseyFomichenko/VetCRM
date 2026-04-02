# SQL-миграции VetCRM (ручной запуск в pgAdmin 4)

## Вариант 1: полностью пустая база

Выполните один файл **000_Full_Schema.sql** — создаёт таблицы `Clients` и `pets` со всеми колонками.

## Вариант 2: пошаговые миграции

Выполняйте скрипты **по порядку** в нужной базе PostgreSQL:

1. **001_Init_Clients.sql** — создаёт таблицу `Clients` (колонка `Id`).
2. **002_Expand_Clients_Table.sql** — добавляет в `Clients` колонки `FullName`, `Phone`, `Email`, `Address`, `Notes`, `Status`, `CreatedAt` и уникальный индекс по `Phone`.
3. **003_Init_Pets.sql** — создаёт таблицу `pets`.
4. **004_Init_Appointments.sql** — создаёт таблицу `Appointments`.
5. **005_Init_MedicalRecords.sql** — создаёт таблицы `MedicalRecords` и `Vaccinations`.
6. **006_Init_Identity.sql** — создаёт таблицы `Users`, `RefreshTokens`, `PasswordResetTokens`.
7. **007_Init_Notifications.sql** — создаёт таблицу `ReminderLogs`.

Если таблица `Clients` уже есть (например, после предыдущего запуска EF-миграций), достаточно выполнить **002** и **003** (в 002 использовано `ADD COLUMN IF NOT EXISTS`, повторный запуск безопасен).
