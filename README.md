# VetCRM

REST API для учёта в ветеринарной клинике: клиенты, питомцы, приёмы, медицинские записи, уведомления и отчёты. Модульный монолит на .NET 8.

**Стек:** .NET 8, C#, PostgreSQL, Entity Framework Core, JWT, модульный монолит.

**Схема модулей:** [docs/modules-diagram.md](docs/modules-diagram.md).

---

## Быстрый старт

1. **Docker (Postgres + API):** из корня репозитория:  
   `docker compose up -d --build`  
   Поднимаются PostgreSQL 18.1 (порт 5432, данные в volume `postgres_data`) и образ API на **http://localhost:8080**.  
   Секреты и строка подключения задаются переменными окружения (см. `.env.example`); Compose подставляет значения по умолчанию для локальной разработки.

2. **Только БД:** `docker compose up -d postgres` — если API запускаете через `dotnet run`.

3. **Миграции:** создать схему БД, выполнив SQL-скрипты из `docs/sql` в порядке нумерации (или один `000_Full_Schema.sql`, затем при необходимости остальные по списку в [docs/sql/README.md](docs/sql/README.md)).

4. **Запуск API без Docker:** из корня репозитория:  
   `dotnet run --project VetCRM.Api`  
   Порты — в `VetCRM.Api/Properties/launchSettings.json`.

5. **Swagger:** при `docker compose` с сервисом `api` в Development: `http://localhost:8080/swagger`. При `dotnet run` — `https://localhost:<port>/swagger` из вывода консоли или `launchSettings.json`.

6. **Тестовые данные (опционально):** в окружении Development вызвать  
   `POST /api/dev/seed`  
   Будет создан пользователь Admin и тестовые клиенты, питомцы, приёмы и медзаписи.  
   **Учётные данные Admin для входа в Swagger:**  
   - Email: `admin@vetcrm.local`  
   - Пароль: `Admin123!`  
   После логина через `POST /api/auth/login` подставьте полученный `accessToken` в Authorize в Swagger и вызывайте защищённые эндпоинты.

---

## Интеграционные тесты

Проект **VetCRM.IntegrationTests** содержит три сценария «полного прохода» на реальной БД:

1. Клиент → питомец: создание клиента, питомца с привязкой к клиенту, получение клиента и питомца по id (с JWT).
2. Регистрация → логин → защищённый API: регистрация пользователя, логин, вызов `GET /api/clients` с Bearer-токеном.
3. Приём → завершение → медзапись: создание клиента и питомца, приёма, завершение приёма с медзаписью, проверка `GET /api/pets/{petId}/medical-records`.

**Предусловие:** БД PostgreSQL поднята (например, `docker compose up -d`), схема создана (скрипты из `docs/sql`). Строка подключения по умолчанию: `Host=localhost;Port=5432;Database=vetcrm;Username=postgres;Password=postgres`. Переменная окружения `VetCRM_Test_ConnectionString` переопределяет её.

Запуск: `dotnet test VetCRM.IntegrationTests/VetCRM.IntegrationTests.csproj`
