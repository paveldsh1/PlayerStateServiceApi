# PlayerState Service API

Я — микросервис для сохранения и выдачи состояния игрока, который использует внешнее приложение (например, Shooter). Храню последнюю позицию и угол игрока по его `nickname`.

---

## Что я делаю
- Принимаю сохранение состояния по `PUT /playerstates/{nickname}`.
- Отдаю сохраненное состояние по `GET /playerstates/{nickname}`.
- Использую SQLite через EF Core; база создается при старте.
- Не требую авторизации (на текущий момент).

## Стек технологий
- C# / .NET 8 (ASP.NET Core Web SDK)
- ASP.NET Core minimal API
- Entity Framework Core 8
- EF Core SQLite provider
- SQLite (локальная база)

## Быстрый старт
```bash
dotnet run --project PlayerStateServiceApi/PlayerStateServiceApi.csproj
```
По умолчанию запускаюсь на `http://localhost:51360` (см. `Properties/launchSettings.json`).

## Конфигурация
`appsettings.json`:
- `ConnectionStrings:DefaultConnection` — строка подключения к SQLite (по умолчанию `Data Source=playerstate.db`).

## API

### GET /playerstates/{nickname}
Возвращает сохраненное состояние.

- `200 OK` + JSON состояния
- `404 Not Found` если записи нет
- `400 Bad Request` если `nickname` пустой/из пробелов

Пример ответа:
```json
{
  "nickname": "Alex",
  "playerX": 10.5,
  "playerY": -3.2,
  "playerA": 1.57,
  "updatedAt": "2026-02-05T12:34:56.789Z"
}
```

### PUT /playerstates/{nickname}
Создает или обновляет состояние.

Пример запроса:
```json
{
  "playerX": 10.5,
  "playerY": -3.2,
  "playerA": 1.57
}
```

- `200 OK` + JSON состояния после сохранения
- `400 Bad Request` если `nickname` пустой/из пробелов

### Формат ошибки
```json
{ "error": "Nickname is required" }
```

## Модель данных
- `nickname` (string) — ключ записи
- `playerX`, `playerY` (float) — позиция игрока
- `playerA` (float) — угол/направление
- `updatedAt` (UTC) — время последнего обновления

## Архитектура
- ASP.NET Core minimal API
- EF Core + SQLite
- База создается при старте (`EnsureCreated`)

### Структура проекта
```
PlayerStateServiceApi/
  Program.cs               endpoints и DI
  Data/PlayerStateContext.cs
  Models/PlayerState.cs
  appsettings.json
```

## Пример cURL
```bash
curl http://localhost:51360/playerstates/Alex

curl -X PUT http://localhost:51360/playerstates/Alex \
  -H "Content-Type: application/json" \
  -d "{ \"playerX\": 10.5, \"playerY\": -3.2, \"playerA\": 1.57 }"
```

## Интеграция с Shooter
- Ожидаемые endpoints: `GET/PUT /playerstates/{nickname}`.
- Базовый URL на стороне игры: `PlayerStateApi:BaseUrl` (по умолчанию `http://localhost:51360`).
- Сервис можно запускать параллельно с игрой на том же компьютере.
