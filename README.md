# CrossApp
Наскрізний проєкт з крос-платформного програмування.
Предметна область: Склад. Сутності: Product, StockBatch, Warehouse, Movement.
Призначення: облік залишків товарів по партіях.

## Запуск
dotnet build
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