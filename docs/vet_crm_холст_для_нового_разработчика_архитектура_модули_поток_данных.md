# VetCRM — быстрый вход для нового разработчика

Этот холст — «карта проекта»: что это за система, как она устроена, где проходят границы ответственности, и как добавлять функционал, не ломая архитектуру.

---

## 1) Что мы строим

**VetCRM** — демонстрационный backend‑проект (портфолио) для ветеринарной клиники: учёт клиентов (владельцев), питомцев, записей/приёмов, мед. записей и уведомлений.

Цель проекта:
- показать **умение проектировать систему**, а не просто CRUD;
- продемонстрировать **архитектуру, границы модулей, контракты, обработку ошибок**;
- иметь живое API, которое рекрутер может открыть в браузере (Swagger).

Не цель:
- микросервисы/распределённость;
- максимальная функциональность и «enterprise‑framework».

---

## 2) Выбранная архитектура

### 2.1 Модульный монолит
Мы используем **модульный монолит**: это один деплой/одно приложение, но логика разделена на независимые модули.

Ключевая идея:
- **каждый модуль** — отдельный проект в solution;
- модуль имеет собственные слои (**Domain / Application / Infrastructure**);
- общение между модулями — **только через контракты (interfaces)**;
- **VetCRM.Api** — единственный HTTP‑адаптер и Composition Root.

### 2.2 Структура решения

```
VetCRM.sln
│
├── VetCRM.Api                    # HTTP слой, DI composition root
│
├── VetCRM.SharedKernel           # Общие базовые вещи (исключения/примитивы)
│
├── VetCRM.Modules.Identity
├── VetCRM.Modules.Clients
├── VetCRM.Modules.Pets
├── VetCRM.Modules.Appointments
├── VetCRM.Modules.MedicalRecords
├── VetCRM.Modules.Notifications
│
└── VetCRM.Infrastructure         # (опционально/позже) общая инфраструктура
```

---

## 3) Базовые правила по слоям

### 3.1 Domain
**Domain** — бизнес‑модель и инварианты.
- Сущности/агрегаты (`Pet`, позже `Client`, `Appointment`…)
- Доменные правила и методы изменения состояния
- Доменные исключения (наследники `DomainException`)

Запрещено:
- EF Core, DbContext
- HTTP, ASP.NET
- DI контейнер
- прямые вызовы других модулей

### 3.2 Application
**Application** — use cases (сценарии) и оркестрация.
- Commands/Queries и их handlers
- Вызов контрактов других модулей
- Решение «что делаем» (создать/изменить/проверить)

Запрещено:
- HTTP/Controller
- DbContext/EF напрямую

**Стандарт типов**:
- `Command`, `Query`, `Result` — **record**
- `Handler` — **class**
- `Entity/Aggregate` — **class**

### 3.3 Infrastructure
**Infrastructure** — техническая реализация.
- EF Core DbContext
- конфигурации сущностей (Fluent API)
- реализации репозиториев
- реализации межмодульных сервисов

Запрещено:
- бизнес‑правила
- зависимость от `VetCRM.Api`

### 3.4 API
**VetCRM.Api** — адаптер HTTP → Application.
- DTO для запросов/ответов
- Controllers (тонкие)
- Swagger
- Global exception middleware
- DI регистрация модулей

Запрещено:
- бизнес‑логика

---

## 4) Границы между модулями

### 4.1 Контракты
Общение между модулями происходит через **интерфейсы** в `Application.Contracts` модуля‑владельца.

Пример (уже реализовано):
- `VetCRM.Modules.Clients.Application.Contracts.IClientReadService`

Важно:
- **контракт объявляет модуль‑владелец**, не потребитель
- нельзя дублировать один и тот же интерфейс в двух модулях (это уже ловили как баг)

---

## 5) Текущее состояние (зафиксировано)

### 5.1 Модуль Pets — вертикальный срез готов
Реализован use case **CreatePet** от API до БД.

#### Структура модуля Pets
```
VetCRM.Modules.Pets
│
├── Domain
│   ├── Pet.cs
│   ├── PetStatus.cs
│   └── Events/
│
├── Application
│   ├── Commands/
│   │   └── CreatePet/
│   ├── Queries/
│   └── Contracts/
│       └── IPetRepository.cs
│
├── Infrastructure
│   ├── PetsDbContext.cs
│   ├── Configurations/
│   │   └── PetConfiguration.cs
│   └── Repositories/
│       └── PetRepository.cs
│
└── Module.cs
```

#### Бизнес‑правила Pets ↔ Clients (приняты)
**Вариант A** (жизненная модель):
1) `Pet` может существовать без клиента (ClientId опционален)
2) если `ClientId` указан — клиент **обязан существовать**
3) если клиента удалили — pet остаётся, `ClientId` становится `null`

Это правило живёт в **Application handler** как оркестрация (проверка существования клиента через контракт).

### 5.2 Модуль Clients — минимальный контракт и инфраструктура
На данный момент для потребностей Pets реализован минимальный контракт:
- `IClientReadService.ExistsAsync(Guid clientId, CancellationToken)`

Реализация находится в `Clients.Infrastructure` (через EF Core).

Важно: Clients может быть «тонким» на старте — модуль может существовать как **контракт + инфраструктура**, домен появится, когда будут инварианты.

---

## 6) Поток данных: CreatePet (как сейчас работает)

### 6.1 HTTP запрос
`POST /api/pets`

### 6.2 API слой
- принимает `CreatePetRequest` (HTTP DTO)
- маппит DTO → `CreatePetCommand`
- вызывает `CreatePetHandler`

Важно: в API DTO используем **DateTime**, а в домене/команде можно использовать **DateOnly** (либо унифицировать). Ранее ловили проблему с `DateOnly` в JSON.

### 6.3 Application слой
`CreatePetHandler`:
- если `ClientId == null` → проверка клиента не нужна
- если `ClientId != null`:
  - вызывает `IClientReadService.ExistsAsync`
  - если `false` → бросает **ClientNotFoundException** (не `InvalidOperationException`)
- создаёт `Pet` (агрегат)
- сохраняет через `IPetRepository`

### 6.4 Infrastructure слой
- `PetRepository` сохраняет сущность через EF Core
- DbContext/конфигурации управляют схемой

---

## 7) Ошибки и их отображение в HTTP

### 7.1 Правило исключений
- Для бизнес‑ошибок бросаем **доменные/прикладные исключения**, а не системные
- Запрещено бросать `InvalidOperationException` как бизнес‑ошибку

### 7.2 Global exception middleware
В `VetCRM.Api` подключён middleware, который маппит исключения на HTTP:
- `ClientNotFoundException` → **404**
- `DomainException` → **400**
- любые другие исключения → **500**

Отдельно: ошибки model binding/валидации ASP.NET (не дошли до контроллера) возвращают **400** автоматически.

---

## 8) Инфраструктура и БД

### 8.1 PostgreSQL
БД запускается локально (рекомендуется Docker):
- порт `5432`
- connection string в `VetCRM.Api/appsettings.json`

### 8.2 EF Core migrations
Критично: таблицы появляются только после применения миграций.
Мы уже ловили ошибку вида `relation "Clients" does not exist` — это означает «нет миграций/таблиц».

Принцип:
- у каждого модуля свой DbContext и свои миграции
- но все миграции применяются к одной БД

---

## 9) DI и подключение модулей

### 9.1 Module.cs
Каждый модуль предоставляет extension‑метод подключения (например, `AddPetsModule`, `AddClientsModule`).

`Module.cs` — **единственная точка входа модуля** для остального приложения.

### 9.2 Program.cs
`VetCRM.Api` подключает модули в фазе регистрации сервисов (до `Build()`):
- `AddClientsModule(connectionString)`
- `AddPetsModule(connectionString)`

Порядок Add* как правило не критичен, но логика группировки соблюдается:
1) чтение конфигурации/строки подключения
2) регистрация модулей
3) AddControllers/Swagger
4) middleware pipeline

---

## 10) Стандарты кода и договорённости

### 10.1 Именование
- Команды/квери: `CreatePetCommand`, `GetPetByIdQuery`
- Хендлеры: `CreatePetHandler`
- Результаты: `CreatePetResult`

### 10.2 Records
- Вход/выход use case: records
- Хендлеры: class

### 10.3 Контракты
- контракты межмодульные: `VetCRM.Modules.<Owner>.Application.Contracts`
- реализации: `VetCRM.Modules.<Owner>.Infrastructure`

---

## 11) Что делать дальше (roadmap ближайших шагов)

### Следующий обязательный шаг
**Unit‑тесты для `CreatePetHandler`**.

Минимальный набор тестов (4 штуки):
1) `ClientId = null` → pet создаётся, `IClientReadService` не вызывается
2) `ClientId != null`, `ExistsAsync = true` → pet создаётся
3) `ClientId != null`, `ExistsAsync = false` → `ClientNotFoundException`
4) при ошибке клиент не найден → `IPetRepository.AddAsync` не вызывается (нет side effects)

### Далее
- добавить `CreateClient` в Clients
- read‑use cases (GetPetById, search)
- авторизация/роли (Identity)

---

## 12) Как быстро разобраться в коде новому разработчику

**1) Начни с `VetCRM.Api/Program.cs`**
- какие модули подключаются
- как устроена middleware‑цепочка

**2) Посмотри `VetCRM.Modules.Pets/Application/Commands/CreatePet`**
- это эталонный use case

**3) Посмотри `VetCRM.Modules.Clients/Application/Contracts`**
- как устроены межмодульные зависимости

**4) Посмотри `Infrastructure` в обоих модулях**
- DbContext
- репозитории
- конфигурации

**5) Ошибки — в middleware + domain/application exceptions**

---

## 13) Частые ошибки, которые уже встречали

- Дублировать один и тот же контракт в разных модулях → DI/архитектура ломается
- Использовать системные исключения (`InvalidOperationException`) как бизнес‑ошибки → middleware отдаёт 500
- Забыть поднять Postgres или не применить миграции → 500 с ошибками подключения/`relation does not exist`
- Пытаться использовать `DateOnly` напрямую в API DTO без конвертера → 400 model binding

---

Если нужна «карта» конкретных файлов/папок по текущему состоянию репозитория — добавим отдельный раздел с paths и ссылками на ключевые классы.

