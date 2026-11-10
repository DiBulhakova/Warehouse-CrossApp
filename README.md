# CrossApp
Наскрізний проєкт з крос-платформного програмування.<br>
Предметна область: Склад. Сутності: Product, StockBatch, Warehouse, Movement.<br>
Призначення: облік залишків товарів по партіях.

## Запуск
```bash
dotnet build 
dotnet run --project src/Cli
```

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
```bash
dotnet build
dotnet run --project src/Cli
dotnet publish src/Cli -c Release -r osx-x64 --self-contained true
```

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

Це демонструє принцип: SDK створює застосунок, Runtime його виконує.<br>

## Доменні інваріанти (лабораторна 4)

### Product
- Id, Sku, Name — не порожні (ArgumentException)
- Quantity — не від'ємна при створенні (ArgumentOutOfRangeException)
- RegisterArrival/Issue — amount > 0 (ArgumentOutOfRangeException)
- Issue — не можна видати більше, ніж є на залишку (InvalidOperationException)

### Warehouse
- Id, Name — не порожні (ArgumentException)

### Movement
- Id — не порожній (ArgumentException)
- Amount > 0 (ArgumentOutOfRangeException)
- Перехід статусу лише за дозволеною матрицею: Draft → Confirmed / Cancelled (InvalidOperationException)

### Міжсутнісний інваріант (додаткове завдання)
- Неможливо зареєструвати прихід товару на закритий склад (StockOperations.RegisterArrival),<br>
  правило винесене в окремий клас-сервіс, а не в Product чи Warehouse<br><br>

Запуск: `dotnet run --project src/Cli -f net10.0`

## Лабораторна 5: сервісний шар, два сховища, ручний DI

Домен: Склад. Головна сутність: `Product` (лаб. 4).

### Архітектура
- Інтерфейс сховища: `ICatalogStore` (`src/Core/Abstractions/ICatalogStore.cs`) із методами `List`, `GetById`, `Add`, `Update`, `Remove`.
- Реалізації:
  - `InMemoryCatalogStore`: `Dictionary` у пам'яті, початкові дані з `SampleData` (16 записів), після виходу дані зникають.
  - `FileCatalogStore`: JSON-файл `data/catalog.json`, кеш у пам'яті, завантаження при першому зверненні, запис на диск після кожної зміни. <br>
На диск іде `ProductDto`, мапінг через `Product.ToDto/FromDto`.
  - `CachingCatalogStore`: декоратор над будь-яким `ICatalogStore`, кешує `List()` і скидає кеш при змінах (додаткове завдання).
- Сервіс: `CatalogService` (`src/Core/Services/CatalogService.cs`) із операціями `Add`, `Receive`, `Issue`, `All`, `Find`, `Search(Func<Product,bool>)`. <br>
Залежить лише від `ICatalogStore`, залежність передається через конструктор.
- Composition root: `src/Cli/Program.cs`. Тут єдине місце, де створюються конкретні класи сховищ. Альтернативний варіант: `src/Cli/StoreFactory.cs`.
- DI-контейнер (`Microsoft.Extensions.DependencyInjection`) не використовується, залежності вручну через `new`. Контейнер з'явиться на тижні 10.<br><br>

Схема залежностей: `Cli → CatalogService → ICatalogStore → InMemoryCatalogStore | FileCatalogStore`.

### Запуск
```bash
dotnet run --project src/Cli -f net10.0                    # сховище в пам'яті 
dotnet run --project src/Cli -f net10.0 -- --file          # файлове сховище
dotnet run --project src/Cli -f net10.0 -- --cache         # з декоратором кешу
dotnet run --project src/Cli -f net10.0 -- --file --cache  # файл + кеш
```

Файл даних створюється за шляхом `src/Cli/bin/Debug/net10.0/data/catalog.json`. Повторні запуски з `--file` накопичують записи.

### Самоперевірка
- `FileCatalogStore` згадується в `src/Core` лише у власному файлі.
- `Console.` у `src/Core` відсутній.
- Конструктор `CatalogService` приймає `ICatalogStore`, а не клас.