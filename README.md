# CrossApp
Наскрізний проєкт з крос-платформного програмування.<br>
Предметна область: Склад. Сутності: Product, StockBatch, Warehouse, Movement.<br>
Призначення: облік залишків товарів по партіях.

## Запуск
dotnet build <br>
dotnet run --project src/Cli

## Середовище
.NET SDK 10.0.100, macOS 15.6.1 (Sequoia), Intel x64 (osx-x64)

## Порівняння self-contained публікації

| RID        | Розмір publish |
|------------|----------------|
| osx-x64    | 76 MB          |
| linux-x64  | 79 МВ          |

Розмір близький для обох RID, оскільки self-contained публікація завжди
включає копію .NET runtime разом із застосунком.

## Структура solution
CrossApp/<br>
  src/<br>
    Core/   — бібліотека: збір інформації про середовище<br>
    Cli/    — консольний клієнт, форматує вивід

## Команди
dotnet build<br>
dotnet run --project src/Cli<br>
dotnet publish src/Cli -c Release -r osx-x64 --self-contained true

## Порівняння публікації

| RID      | Режим               | Розмір publish | Потрібен runtime |
|----------|---------------------|----------------|-------------------|
| osx-x64  | self-contained      | ~70 МБ         | ні                |
| osx-x64  | framework-dependent | ~0.2 МБ        | так (.NET 10)     |

## Multi-targeting

Проєкти Core і Cli зібрані під два TFM: net8.0 та net10.0<br>
(`<TargetFrameworks>net8.0;net10.0</TargetFrameworks>`).<br>
Компіляція під обидва TFM проходить успішно.<br><br>

Запуск Cli під net8.0 неможливий: на машині встановлено лише .NET 10 Runtime (10.0.12),<br>
а запуск net8.0-застосунку вимагає окремо встановленого .NET 8 Runtime, якого немає.<br>
SDK 10.0.401 дозволяє компілювати код під різні TFM, але не гарантує можливості його<br>
запуску — для цього потрібен відповідний Runtime на цільовій машині.<br><br>

Це демонструє принцип: SDK створює застосунок, Runtime його виконує.<br><br>

Запуск: `dotnet run --project src/Cli -f net10.0`