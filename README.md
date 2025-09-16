# BreakFree — трекер звичок (Core + WPF)

**Stack:** .NET 8, EF Core + SQLite, ADO.NET, WPF (Windows UI).  
**Кросплатформа:** усе ядро збирається і тестується на macOS/Linux/Windows; WPF — лише Windows.

## Структура
```
/src/BreakFree.Core/     — моделі, DbContext, сервіси, ADO.NET-звіти
/src/BreakFree.Wpf/      — WPF UI (посилається на Core)
/tests/BreakFree.Tests/  — xUnit тести (кросплатформа)
```

## Швидкий старт

### Mac / Linux (ядро та тести)
```bash
dotnet restore
dotnet build ./src/BreakFree.Core -c Debug
dotnet test ./tests/BreakFree.Tests -c Debug
```

### Windows (WPF + ядро)
Відкрий `BreakFree.sln` у Visual Studio 2022 і натисни **F5**.  
Або CLI:
```bash
dotnet restore
dotnet build ./src/BreakFree.Wpf/BreakFree.Wpf.csproj -c Debug
```

## EF Core міграції
```bash
cd src/BreakFree.Core
dotnet ef migrations add Init
dotnet ef database update
```

> SQLite база `breakfree.db` створюється в робочій директорії процесу.

## CI
- Job **core** (ubuntu + macos): збирає `BreakFree.Core` і ганяє тести.
- Job **wpf** (windows): збирає `BreakFree.Wpf` і ганяє тести.

## Ліцензія
MIT
