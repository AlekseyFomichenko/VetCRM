# VetCRM — финальная картина проекта

Этот холст описывает, **каким VetCRM должен быть в конце разработки**: функционал, модули, UX API, роли, данные, архитектурные правила, инфраструктура и критерии готовности. Это «north star» документация для команды.

---

## 1) Что такое VetCRM в финале

**VetCRM** — веб‑API для CRM/учёта в ветеринарной клинике:
- клиенты (владельцы животных)
- питомцы
- расписание и записи на приём
- медицинские записи (осмотр, диагноз, лечение, вакцинации)
- напоминания (вакцинации и др.)
- отчёты (операционные метрики)

Система рассчитана на 3 роли:
- **Admin** — управление пользователями, настройками, доступами
- **Veterinarian** — мед. записи, завершение приёма, просмотр истории
- **Receptionist (администратор ресепшн)** — клиенты/питомцы, создание/перенос/отмена записей

Внешний интерфейс:
- **Swagger** для демонстрации рекрутерам
- (опционально) фронтенд от знакомого на TS (тут **не привязываемся к API в задачах**, но API проектируется стабильно)

---

## 2) Нефункциональные цели (почему проект выглядит «взрослым»)

1) **Чёткие границы модулей** (модульный монолит)
2) **Единый стиль ошибок**: бизнес‑ошибки → корректные HTTP
3) **Тесты**: unit на use cases + несколько интеграционных тестов
4) **Надёжная инфраструктура**: Postgres, Redis, миграции, фоновые задачи
5) **Документация**: README, quickstart, diagram, swagger examples

---

## 3) Архитектура в финале

### 3.1 Модульный монолит
Один деплой, но код разделён на независимые модули.

Solution:
```
VetCRM.sln
│
├── VetCRM.Api
├── VetCRM.SharedKernel
│
├── VetCRM.Modules.Identity
├── VetCRM.Modules.Clients
├── VetCRM.Modules.Pets
├── VetCRM.Modules.Appointments
├── VetCRM.Modules.MedicalRecords
├── VetCRM.Modules.Notifications
├── VetCRM.Modules.Reports
│
└── VetCRM.Infrastructure (опционально)
```

### 3.2 Внутренний шаблон модуля
Каждый `VetCRM.Modules.*` имеет одинаковый «скелет»:

- **Domain**: сущности, инварианты, доменные исключения
- **Application**: use cases (Commands/Queries), контракты
- **Infrastructure**: EF Core DbContext, конфигурации, репозитории, реализации контрактов
- **Module.cs**: регистрация DI (единственная точка входа модуля)

### 3.3 Стандарты типов
- Command/Query/Result — `record`
- Handler — `class`
- Entity/Aggregate — `class`
- VO — `record`

---

## 4) Границы ответственности

### 4.1 Domain
- не знает HTTP/Swagger
- не знает EF Core
- не знает DI
- не знает другие модули

### 4.2 Application
- orchestrates use case
- вызывает только **контракты** других модулей
- кидает только **доменные/прикладные исключения** (наследники DomainException)
- не возвращает IActionResult

### 4.3 Infrastructure
- только техника: EF, Redis, фоновые реализации
- никаких бизнес‑правил

### 4.4 API
- тонкие контроллеры
- DTO только для HTTP
- глобальный Exception middleware
- auth middleware

---

## 5) Данные и доменные модели (в финале)

### 5.1 Clients
**Client** — владелец животных.

Поля (минимум):
- Id
- FullName
- Phone (уникальный в пределах клиники)
- Email (опционально)
- Address (опционально)
- Notes (опционально)
- Status (Active/Archived)
- CreatedAt

Бизнес‑правила:
- телефон может быть обязателен (решение продукта)
- soft-delete/архивация

### 5.2 Pets
**Pet** — агрегатный корень Pets.

Поля:
- Id
- ClientId? (опционально)
- Name
- Species (Cat/Dog/… или строка)
- Breed (опционально)
- Sex (опционально)
- BirthDate (в домене DateOnly, в API DateTime)
- Status (Active/Archived)

Принятая модель связи Pets↔Clients (зафиксирована):
1) Pet может существовать без Client
2) если ClientId указан — клиент обязан существовать
3) если клиент удалён — Pet остаётся, ClientId становится null

### 5.3 Appointments
**Appointment** — запись на приём.

Поля:
- Id
- PetId
- ClientId (денормализация/фикс на момент записи — опционально)
- VeterinarianUserId
- StartsAt, EndsAt
- Status: Scheduled/Cancelled/Rescheduled/Completed/NoShow
- Reason/Comment
- CreatedByUserId

Бизнес‑правила:
- нельзя пересекать приёмы одного врача (или можно — зависит от требований; в финале делаем запрет)
- перенос = новый слот + история изменений

### 5.4 MedicalRecords
**MedicalRecord** — мед. запись, создаётся при завершении приёма.

Поля:
- Id
- AppointmentId
- PetId
- VeterinarianUserId
- Complaint (жалоба)
- Diagnosis
- TreatmentPlan
- Prescription (медикаменты/дозировки)
- Attachments (опц.)
- CreatedAt

**Vaccination** (подсущность/таблица):
- Id
- MedicalRecordId
- VaccineName
- VaccinationDate
- NextDueDate
- Batch/Manufacturer (опц.)

Бизнес‑правила:
- MedicalRecord не существует без Pet
- запись создаётся только из Completed Appointment

### 5.5 Notifications
**ReminderLog** — журнал уведомлений.

Поля:
- Id
- Type (VaccinationDue/AppointmentTomorrow/…)
- Target (ClientId/PetId)
- Channel (Email/SMS/Demo)
- Payload (json)
- Status (Sent/Failed)
- CreatedAt
- Error (опц.)

---

## 6) Роли и безопасность

### 6.1 Аутентификация
- JWT access token
- refresh token (опционально для демо; можно сделать простую схему)

### 6.2 Авторизация
Роли:
- Admin
- Veterinarian
- Receptionist

Матрица доступа (пример финала):
- Clients/Pets: Receptionist+Admin полный CRUD, Vet — read
- Appointments: Receptionist+Admin CRUD, Vet — view schedule + complete
- MedicalRecords: Vet+Admin CRUD (или только create/update), Receptionist — read summary
- Reports: Admin
- Users: Admin

---

## 7) API в финале (что должно быть доступно через Swagger)

### 7.1 Clients
- POST /api/clients
- GET /api/clients?search=
- GET /api/clients/{id}
- PUT /api/clients/{id}
- POST /api/clients/{id}/archive

### 7.2 Pets
- POST /api/pets
- GET /api/pets?search=
- GET /api/pets/{id}
- PUT /api/pets/{id}
- POST /api/pets/{id}/assign-client
- POST /api/pets/{id}/unassign-client

### 7.3 Appointments
- POST /api/appointments
- GET /api/appointments?date=YYYY-MM-DD&vetId=
- PUT /api/appointments/{id}/reschedule
- PUT /api/appointments/{id}/cancel
- PUT /api/appointments/{id}/complete (создаёт medical record)

### 7.4 MedicalRecords
- GET /api/pets/{petId}/medical-records
- GET /api/medical-records/{id}
- PUT /api/medical-records/{id}
- POST /api/medical-records/{id}/vaccinations

### 7.5 Notifications
- POST /api/notifications/send (ручной запуск)
- GET /api/notifications/log

### 7.6 Reports
- GET /api/reports/appointments?from=&to=
- GET /api/reports/overdue-vaccinations

Примечание по DTO:
- API DTO максимально «совместимы» (DateTime, string, Guid)
- Domain может использовать DateOnly и VO

---

## 8) Ошибки и ответы (единый контракт)

В финале все ошибки должны быть предсказуемыми:

- `ClientNotFoundException` → 404
- `PetNotFoundException` → 404
- `AppointmentConflictException` (пересечение) → 409
- `DomainException` (прочие бизнес‑ошибки) → 400
- неизвестное исключение → 500 (с безопасным сообщением)

Формат ответа (ProblemDetails‑like):
- type
- title
- status
- detail (опц.)
- traceId (полезно для дебага)

---

## 9) Фоновые задачи и Redis

### 9.1 Redis
Использование в финале:
- кэширование справочных/частых чтений (по желанию)
- distributed lock для фоновых задач (если нужно)

### 9.2 Background jobs
Простая фонова задача напоминаний:
- периодически проверяет вакцинации с NextDueDate <= (сегодня + N дней)
- создаёт ReminderLog
- отправляет через демо Email/SMS сервис (можно «заглушки»)

Инструмент:
- HostedService / BackgroundService
- (опционально) Hangfire/Quartz — но для демо достаточно HostedService

---

## 10) Тестирование в финале

### 10.1 Unit tests (обязательно)
- каждый use case (handler) тестируется в изоляции
- моки контрактов
- тестируются ветки успеха/ошибок

Пример для CreatePet:
1) ClientId null → создаём, IClientReadService не вызывается
2) ClientId задан, Exists=true → создаём
3) ClientId задан, Exists=false → ClientNotFoundException
4) при ошибке репозиторий не вызывается

### 10.2 Integration tests (минимум)
- 1–2 теста на «полный проход» (API→DB) через TestServer
- можно использовать Testcontainers для Postgres (если не хочется — оставить как опционально)

---

## 11) Документация и демонстрация рекрутерам

В финале должны быть:
- README: описание проекта, архитектура, модули, quickstart
- Схема модулей + поток CreatePet
- Swagger примеры запросов
- Инструкции:
  - запуск Postgres (Docker)
  - миграции
  - запуск API

Бонусы для «вау»:
- seed‑данные (скрипт/endpoint только для DEV)
- коллекция Postman (опционально)

---

## 12) Критерии готовности финала (Definition of Done)

Проект считается «готовым к показу», когда:

1) API запускается одной командой (docker compose + dotnet run)
2) Есть полноценные модули:
   - Clients
   - Pets
   - Appointments
   - MedicalRecords
   - Notifications
   - Identity
3) Есть централизованный error handling
4) Есть базовые unit‑тесты на ключевые use cases
5) Swagger полностью отражает функциональность
6) Есть документация (README + quickstart)

---

## 13) Как расширять проект, не ломая архитектуру

Правила:
- новый функционал начинается с **use case** (Application)
- Domain меняем только когда появляются инварианты
- межмодульные зависимости — только через Contracts владельца
- API остаётся тонким
- ошибки — только доменные/прикладные + middleware
- миграции — по модулям

---

## 14) Рекомендуемый порядок реализации (дорожная карта)

1) Закончить Pets вертикаль: **unit‑тесты**
2) Clients: CreateClient + поиск
3) Appointments: создание/перенос/отмена/расписание
4) MedicalRecords: завершение приёма → medical record + vaccination
5) Notifications: background reminders + log
6) Identity: JWT + роли
7) Reports: 2–3 отчёта
8) Полировка: README, seed, интеграционные тесты

