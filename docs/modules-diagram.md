# Схема модулей VetCRM

Структура модулей и зависимости между ними (точка входа — Api, межмодульное общение только через контракты Application).

```mermaid
flowchart TB
  subgraph Api [VetCRM.Api]
    API[Program.cs + Controllers]
  end

  subgraph Modules [Модули]
    Appointments[Appointments]
    Clients[Clients]
    Identity[Identity]
    MedicalRecords[MedicalRecords]
    Notifications[Notifications]
    Pets[Pets]
    Reports[Reports]
  end

  Shared[SharedKernel]

  API --> Appointments
  API --> Clients
  API --> Identity
  API --> MedicalRecords
  API --> Notifications
  API --> Pets
  API --> Reports
  API --> Shared

  Pets --> Clients
  Pets --> Shared

  Appointments --> Clients
  Appointments --> MedicalRecords
  Appointments --> Pets
  Appointments --> Shared

  Notifications --> Clients
  Notifications --> MedicalRecords
  Notifications --> Pets

  Reports --> Appointments
  Reports --> Clients
  Reports --> MedicalRecords
  Reports --> Pets
  Reports --> Shared

  Identity --> Shared
  MedicalRecords --> Shared
  Clients --> Shared
```

Подробнее о контрактах и принципах — [architecture-dependencies.md](architecture-dependencies.md).
