# Описание контроллеров API VetCRM

Документ описывает все контроллеры: маршруты, авторизацию, входные данные и ответы.

---

## 1. AuthController — аутентификация

**Базовый маршрут:** `api/auth`  
**Авторизация:** все эндпоинты публичные (без JWT).

| Метод | Маршрут | Назначение |
|-------|---------|------------|
| POST | register | Регистрация пользователя (роли Veterinarian или Receptionist) |
| POST | login | Вход по email и паролю, выдача access- и refresh-токенов |
| POST | refresh | Обновление access-токена по refresh-токену |
| POST | forgot-password | Запрос сброса пароля (отправка токена на email) |
| POST | reset-password | Сброс пароля по токену и новому паролю |

### POST /api/auth/register

**Назначение:** самостоятельная регистрация пользователя (роль Admin недоступна).

**Тело запроса (RegisterRequest):**

| Поле | Тип | Описание |
|------|-----|----------|
| email | string | Email (уникальный) |
| password | string | Пароль |
| role | UserRole | 1 = Veterinarian, 2 = Receptionist |

**Ответ:** 201 Created, тело: `{ "userId": "guid" }`.

---

### POST /api/auth/login

**Назначение:** вход в систему, получение JWT и refresh-токена.

**Тело запроса (LoginRequest):**

| Поле | Тип | Описание |
|------|-----|----------|
| email | string | Email |
| password | string | Пароль |

**Ответ:** 200 OK, тело (LoginResponse):

| Поле | Тип | Описание |
|------|-----|----------|
| accessToken | string | JWT для заголовка Authorization |
| refreshToken | string | Токен для обновления access |
| expiresAt | DateTime | Срок действия refresh-токена |
| userId | Guid | Id пользователя |
| email | string | Email |
| role | string | Роль (Admin, Veterinarian, Receptionist) |

---

### POST /api/auth/refresh

**Назначение:** обновление access-токена без повторного ввода пароля.

**Тело запроса (RefreshTokenRequest):** `{ "refreshToken": "string" }`.

**Ответ:** 200 OK, тело — тот же формат, что и у login (LoginResponse).

---

### POST /api/auth/forgot-password

**Тело запроса (ForgotPasswordRequest):** `{ "email": "string" }`.  
**Ответ:** 204 No Content.

---

### POST /api/auth/reset-password

**Тело запроса (ResetPasswordRequest):**

| Поле | Тип | Описание |
|------|-----|----------|
| token | string | Токен из письма/процесса forgot-password |
| newPassword | string | Новый пароль |

**Ответ:** 204 No Content.

---

## 2. ClientsController — клиенты

**Базовый маршрут:** `api/clients`  
**Авторизация:** JWT, роли Admin, Receptionist, Veterinarian (уточнения по эндпоинтам ниже).

| Метод | Маршрут | Назначение | Роли |
|-------|---------|------------|------|
| POST | / | Создание клиента | Admin, Receptionist |
| GET | / | Список клиентов с поиском и пагинацией | все три |
| GET | /{id} | Клиент по id | все три |
| PUT | /{id} | Обновление клиента | все три |
| POST | /{id}/archive | Архивация клиента | Admin, Receptionist |

### POST /api/clients

**Тело запроса (CreateClientRequest):**

| Поле | Тип | Описание |
|------|-----|----------|
| fullName | string | ФИО |
| phone | string | Телефон (уникальный) |
| email | string? | Email |
| address | string? | Адрес |
| notes | string? | Заметки |

**Ответ:** 201 Created, заголовок Location, тело: `{ "clientId": "guid" }`.

---

### GET /api/clients

**Query-параметры:** `search` (string?), `page` (int, по умолчанию 1), `pageSize` (int, по умолчанию 20, макс. 100), `status` (ClientStatus? — Active/Archived).

**Ответ:** 200 OK, тело (GetClientsResponse):

| Поле | Тип | Описание |
|------|-----|----------|
| items | ClientResponse[] | Элементы страницы |
| totalCount | int | Общее количество |
| page | int | Номер страницы |
| pageSize | int | Размер страницы |

**ClientResponse:** id, fullName, phone, email, address, notes, status (Active/Archived), createdAt.

---

### GET /api/clients/{id}

**Ответ:** 200 OK, тело — ClientResponse (см. выше). При отсутствии — 404.

---

### PUT /api/clients/{id}

**Тело запроса (UpdateClientRequest):** те же поля, что у CreateClientRequest (fullName, phone, email, address, notes).  
**Ответ:** 200 OK.

---

### POST /api/clients/{id}/archive

**Ответ:** 200 OK (клиент переводится в статус Archived).

---

## 3. PetsController — питомцы

**Базовый маршрут:** `api/pets`  
**Авторизация:** JWT, роли Admin, Receptionist, Veterinarian.

| Метод | Маршрут | Назначение | Роли |
|-------|---------|------------|------|
| POST | / | Создание питомца | Admin, Receptionist |
| GET | /{id} | Питомец по id (заглушка) | все три |

### POST /api/pets

**Тело запроса (CreatePetRequest):**

| Поле | Тип | Описание |
|------|-----|----------|
| name | string | Кличка |
| species | string | Вид (например, Кот, Собака) |
| birthDate | DateTime? | Дата рождения |
| clientId | Guid? | Id владельца (клиента) |

**Ответ:** 201 Created, заголовок Location, тело: `{ "petId": "guid" }`.

---

### GET /api/pets/{id}

**Ответ:** 200 OK (тело — заглушка).

---

## 4. AppointmentsController — приёмы

**Базовый маршрут:** `api/appointments`  
**Авторизация:** JWT, роли Admin, Receptionist, Veterinarian.

| Метод | Маршрут | Назначение | Роли |
|-------|---------|------------|------|
| POST | / | Создание приёма | Admin, Receptionist |
| GET | / | Список приёмов по дате | все три |
| PUT | /{id}/reschedule | Перенос приёма | Admin, Receptionist |
| PUT | /{id}/cancel | Отмена приёма | все три |
| PUT | /{id}/complete | Завершение приёма и создание медзаписи | Admin, Veterinarian |

### POST /api/appointments

**Тело запроса (CreateAppointmentRequest):**

| Поле | Тип | Описание |
|------|-----|----------|
| petId | Guid | Id питомца |
| clientId | Guid | Id клиента |
| veterinarianUserId | Guid? | Id ветврача |
| startsAt | DateTime | Начало приёма |
| endsAt | DateTime | Окончание приёма |
| reason | string? | Причина/повод |

**Ответ:** 201 Created, тело: `{ "appointmentId": "guid" }`. При пересечении по ветврачу — 409.

---

### GET /api/appointments

**Query-параметры:** `date` (обязательный, формат YYYY-MM-DD), `vetId` (Guid?).

**Ответ:** 200 OK, тело — массив AppointmentResponse:

| Поле | Тип | Описание |
|------|-----|----------|
| id | Guid | Id приёма |
| petId | Guid | Id питомца |
| clientId | Guid | Id клиента |
| veterinarianUserId | Guid? | Id ветврача |
| startsAt | DateTime | Начало |
| endsAt | DateTime | Окончание |
| status | AppointmentStatus | Scheduled, Completed, Cancelled |
| reason | string? | Причина |
| createdByUserId | Guid? | Кто создал |
| createdAt | DateTime | Дата создания |

---

### PUT /api/appointments/{id}/reschedule

**Тело запроса (RescheduleAppointmentRequest):** `startsAt` (DateTime), `endsAt` (DateTime).  
**Ответ:** 200 OK.

---

### PUT /api/appointments/{id}/cancel

**Ответ:** 200 OK (приём переводится в статус Cancelled).

---

### PUT /api/appointments/{id}/complete

**Назначение:** завершение приёма и создание медицинской записи.

**Тело запроса (CompleteAppointmentRequest):**

| Поле | Тип | Описание |
|------|-----|----------|
| complaint | string | Жалоба |
| diagnosis | string | Диагноз |
| treatmentPlan | string | План лечения |
| prescription | string | Назначения |
| attachments | string? | Вложения (текст/ссылка) |

**Ответ:** 200 OK.

---

## 5. MedicalRecordsController — медицинские записи

**Базовый маршрут:** `api/medical-records` и `api/pets/{petId}/medical-records`  
**Авторизация:** JWT, роли Admin, Receptionist, Veterinarian.

| Метод | Маршрут | Назначение | Роли |
|-------|---------|------------|------|
| GET | /api/pets/{petId}/medical-records | Список медзаписей по питомцу | все три |
| GET | /{id} | Медзапись по id | все три |
| PUT | /{id} | Обновление медзаписи | Admin, Veterinarian |
| POST | /{id}/vaccinations | Добавление вакцинации к записи | Admin, Veterinarian |

### GET /api/pets/{petId}/medical-records

**Ответ:** 200 OK, тело — массив MedicalRecordResponse.

**MedicalRecordResponse:** id, appointmentId, petId, veterinarianUserId, complaint, diagnosis, treatmentPlan, prescription, attachments, createdAt, vaccinations (массив VaccinationResponse).

**VaccinationResponse:** id, medicalRecordId, vaccineName, vaccinationDate, nextDueDate, batch, manufacturer.

---

### GET /api/medical-records/{id}

**Ответ:** 200 OK, тело — MedicalRecordResponse. При отсутствии — 404.

---

### PUT /api/medical-records/{id}

**Тело запроса (UpdateMedicalRecordRequest):** complaint, diagnosis, treatmentPlan, prescription, attachments (все string, attachments — опционально).  
**Ответ:** 200 OK.

---

### POST /api/medical-records/{id}/vaccinations

**Тело запроса (AddVaccinationRequest):**

| Поле | Тип | Описание |
|------|-----|----------|
| vaccineName | string | Название вакцины |
| vaccinationDate | DateTime | Дата прививки |
| nextDueDate | DateTime? | Следующая дата (ревакцинация) |
| batch | string? | Серия |
| manufacturer | string? | Производитель |

**Ответ:** 200 OK.

---

## 6. NotificationsController — уведомления

**Базовый маршрут:** `api/notifications`  
**Авторизация:** JWT, только Admin.

| Метод | Маршрут | Назначение |
|-------|---------|------------|
| POST | send | Запуск отправки напоминаний о вакцинациях |
| GET | log | Журнал напоминаний с фильтрами |

### POST /api/notifications/send

**Ответ:** 200 OK, тело (ProcessVaccinationRemindersResponse): `created` (создано записей), `sent` (отправлено), `failed` (ошибки).

---

### GET /api/notifications/log

**Query-параметры:** `type` (ReminderType?), `status` (ReminderStatus?), `from` (DateTime?), `to` (DateTime?).

**Ответ:** 200 OK, тело — массив ReminderLogResponse: id, type, targetClientId, targetPetId, channel, payload, status, createdAt, error.

---

## 7. ReportsController — отчёты

**Базовый маршрут:** `api/reports`  
**Авторизация:** JWT, только Admin.

| Метод | Маршрут | Назначение |
|-------|---------|------------|
| GET | appointments | Приёмы за период |
| GET | overdue-vaccinations | Просроченные/скоро просроченные вакцинации |

### GET /api/reports/appointments

**Query-параметры:** `from` (DateTime), `to` (DateTime), `page` (int, по умолчанию 1), `pageSize` (int, по умолчанию 20, макс. 100). Период ограничен (например, не более 1 года).

**Ответ:** 200 OK, тело (AppointmentsReportResponse): totalCount, items (массив элементов приёма: id, petId, clientId, veterinarianUserId, startsAt, endsAt, status, reason, createdAt), page, pageSize.

---

### GET /api/reports/overdue-vaccinations

**Query-параметры:** `page` (int), `pageSize` (int).

**Ответ:** 200 OK, тело (OverdueVaccinationsReportResponse): totalCount, items (vaccinationId, petId, vaccineName, nextDueDate, isOverdue, clientFullName, clientPhone, clientEmail), page, pageSize.

---

## 8. UsersController — пользователи (администрирование)

**Базовый маршрут:** `api/users`  
**Авторизация:** JWT, только Admin.

| Метод | Маршрут | Назначение |
|-------|---------|------------|
| POST | / | Создание пользователя (в т.ч. Admin) |
| GET | / | Список пользователей с поиском и фильтрами |
| GET | /{id} | Пользователь по id |
| PUT | /{id} | Обновление (имя, роль) |
| POST | /{id}/disable | Отключение учётной записи |

### POST /api/users

**Тело запроса (CreateUserRequest):** email, password, role (UserRole: 0=Admin, 1=Veterinarian, 2=Receptionist), fullName (string?).  
**Ответ:** 201 Created, тело: `{ "userId": "guid" }`.

---

### GET /api/users

**Query-параметры:** `search`, `role`, `status` (UserStatus?), `page`, `pageSize`.  
**Ответ:** 200 OK, тело (GetUsersResponse): items (UserResponse[]), totalCount, page, pageSize. **UserResponse:** id, email, role, fullName, status (Active/Disabled), createdAt.

---

### GET /api/users/{id}

**Ответ:** 200 OK, тело — UserResponse. При отсутствии — 404.

---

### PUT /api/users/{id}

**Тело запроса (UpdateUserRequest):** fullName (string?), role (UserRole?).  
**Ответ:** 204 No Content.

---

### POST /api/users/{id}/disable

**Ответ:** 204 No Content (пользователь переводится в статус Disabled).

---

## 9. DevController — разработка

**Базовый маршрут:** `api/dev`  
**Авторизация:** нет; эндпоинт доступен только в окружении Development.

| Метод | Маршрут | Назначение |
|-------|---------|------------|
| POST | seed | Заполнение БД тестовыми данными (Admin, ветврач, клиенты, питомцы, приёмы, медзаписи) |

### POST /api/dev/seed

**Ответ:** 200 OK — данные созданы. В ином окружении — 404.

---

## 10. Общие замечания

- Заголовок **Authorization:** `Bearer <accessToken>` обязателен для всех защищённых эндпоинтов (кроме Auth и Dev/seed).
- Ошибки возвращаются в виде JSON с полями: type, title, status, detail, traceId.
- Даты/время в запросах и ответах — в формате ISO 8601.
