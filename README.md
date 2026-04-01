<img width="2535" height="482" alt="image" src="https://github.com/user-attachments/assets/b8b3f7ac-dd63-4e43-8d72-d0becae62ff9" />
<img width="2539" height="457" alt="image" src="https://github.com/user-attachments/assets/6d04687d-f2b0-4f6d-9ce1-0b926d94f16f" />


# DeskBooking.CoreWcfSolution

Учебный проект на **CoreWCF + .NET 8 + ASP.NET Core MVC + EF Core + SQLite**.

Тема проекта: **Система бронирования рабочих мест (коворкинг)**.

## Что внутри

- CoreWCF-сервис с документированными контрактами и WSDL
- CRUD по комнатам
- CRUD по бронированиям
- Простая аутентификация через логин/пароль и сессионный токен
- Веб-клиент с админкой на ASP.NET Core MVC
- Бизнес-правила:
  - нельзя бронировать в прошлом;
  - нельзя создавать пересекающиеся бронирования одной комнаты;
  - нельзя удалять комнату, если у неё есть активные будущие бронирования;
  - операции управления комнатами доступны только администратору;
  - сотрудник видит и редактирует только свои бронирования.
- Юнит-тесты для бизнес-логики

## Иерархия решения

- `DeskBooking.Contracts` — сервисные и дата-контракты
- `DeskBooking.Domain` — сущности, интерфейсы репозиториев, хеширование пароля
- `DeskBooking.Application` — бизнес-логика и маппинг
- `DeskBooking.Infrastructure` — EF Core, DbContext, репозитории, seed
- `DeskBooking.ServiceHost` — CoreWCF host
- `DeskBooking.WebClient` — веб-клиент и админка
- `DeskBooking.Tests` — тесты

## Стек

- .NET 8
- CoreWCF 1.8.0
- EF Core 8.0.19
- SQLite
- ASP.NET Core MVC
- xUnit

CoreWCF работает поверх ASP.NET Core, поддерживает публикацию metadata/WSDL через `AddServiceModelMetadata`, а клиентские прокси для WCF в .NET можно генерировать через `dotnet-svcutil`. Для этого решения клиент реализован вручную через WCF `ChannelFactory` и `BasicHttpBinding`. citeturn930226search1turn930226search2turn750775search2turn162442search9

## Как открыть в Visual Studio 2022

1. Распакуй архив.
2. Открой файл `DeskBooking.CoreWcfSolution.sln`.
3. Дождись восстановления пакетов.
4. Если NuGet не восстановился сам:
   - ПКМ по solution
   - **Восстановить пакеты NuGet**

## Что нужно сделать после распаковки

### 1) Создать первую миграцию

Открой **Консоль диспетчера пакетов**:

**Сервис** → **Диспетчер пакетов NuGet** → **Консоль диспетчера пакетов**

Выбери:
- **Проект по умолчанию**: `DeskBooking.Infrastructure`
- **Запускаемый проект**: `DeskBooking.ServiceHost`

Выполни:

```powershell
Add-Migration InitialCreate -Project DeskBooking.Infrastructure -StartupProject DeskBooking.ServiceHost
Update-Database -Project DeskBooking.Infrastructure -StartupProject DeskBooking.ServiceHost
```

EF Core поддерживает миграции как штатный способ развития схемы БД, а SQLite используется как полноценный провайдер EF Core. citeturn751530search0turn751530search1turn751530search4

### 2) Запустить проекты

Поставь несколько запускаемых проектов:

- `DeskBooking.ServiceHost`
- `DeskBooking.WebClient`

В Visual Studio:
- ПКМ по solution
- **Настроить запускаемые проекты**
- выбрать **Несколько запускаемых проектов**
- для `DeskBooking.ServiceHost` — **Запуск**
- для `DeskBooking.WebClient` — **Запуск**

## URL по умолчанию

### CoreWCF host
- `http://localhost:5050/`
- `http://localhost:5050/Services/AuthService.svc?wsdl`
- `http://localhost:5050/Services/RoomService.svc?wsdl`
- `http://localhost:5050/Services/BookingService.svc?wsdl`

### Web client
- `http://localhost:5051/`

## Тестовые пользователи

Seed создаёт двух пользователей:

### Администратор
- email: `admin@room-booking.local`
- password: `Admin123!`

### Сотрудник
- email: `employee@room-booking.local`
- password: `Employee123!`

## Что показать на защите

1. В браузере открыть WSDL одного из сервисов.
2. Войти в веб-клиент под администратором.
3. Создать комнату.
4. Создать бронирование.
5. Показать конфликт пересечения по времени.
6. Показать фильтр доступных комнат.
7. Войти под сотрудником.
8. Показать, что сотрудник видит только свои бронирования и не может менять комнаты.

## Что отвечать по архитектуре

Архитектура — **слоистая**:

- Contracts — описание внешнего SOAP API
- Domain — сущности и абстракции
- Application — бизнес-правила
- Infrastructure — работа с БД
- ServiceHost — транспортный слой CoreWCF
- WebClient — отдельный клиент

Почему подходит:
- просто объясняется;
- хорошо делит ответственность;
- легко масштабируется на другие предметные области;
- удобно клонировать ещё в 3 похожих проекта.

## Что ещё можно улучшить потом

- JWT вместо сессионного токена
- аудит действий
- логирование в файл
- Docker
- PostgreSQL вместо SQLite
- роли и права тоньше, чем Admin/Employee
- отдельная таблица сессий
- soft delete для комнат

## Что я сознательно упростил

- транспорт только `basicHttpBinding`;
- для учебной демонстрации используется `http://localhost`, а не HTTPS;
- аутентификация упрощённая, но пароль хранится не открытым текстом, а в виде PBKDF2-хеша;
- клиент не использует Connected Services, а создаёт WCF-каналы вручную.

WCF Client для современных .NET-проектов поставляется через отдельные пакеты, а в версии 8.0 были удалены старые API, завязанные на конфигурационные имена, поэтому ручная конфигурация `Binding` и `EndpointAddress` в коде здесь даже удобнее. citeturn162442search0turn162442search11
