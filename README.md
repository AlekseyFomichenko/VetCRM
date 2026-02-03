# VetCRM

REST API для учёта в ветеринарной клинике: клиенты, питомцы, приёмы, медицинские записи, уведомления и отчёты. Модульный монолит на .NET 8.

**Стек:** .NET 8, C#, PostgreSQL, Entity Framework Core, JWT, модульный монолит.

**Схема модулей:** [docs/modules-diagram.md](docs/modules-diagram.md).

---

## Быстрый старт

1. **Запустить БД:** из корня репозитория выполнить  
   `docker compose up -d`  
   Поднимается PostgreSQL 18.1, база `vetcrm`, порт 5432.

2. **Миграции:** создать схему БД, выполнив SQL-скрипты из `docs/sql` в порядке нумерации (или один `000_Full_Schema.sql`, затем при необходимости остальные по списку в [docs/sql/README.md](docs/sql/README.md)).

3. **Запуск API:** из корня репозитория:  
   `dotnet run --project VetCRM.Api`  
   По умолчанию API слушает порты из `launchSettings.json` (например, https://localhost:7xxx).

4. **Swagger:** в браузере открыть (после запуска API)  
   `https://localhost:<port>/swagger`  
   (порт указан в выводе `dotnet run` или в `VetCRM.Api/Properties/launchSettings.json`).

5. **Тестовые данные (опционально):** в окружении Development вызвать  
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
