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