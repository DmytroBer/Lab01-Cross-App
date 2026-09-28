# CrossApp
Наскрізний проєкт з крос-платформного програмування.

Предметна область: Склад. Сутності: Product, StockBatch, Warehouse, Movement.
Призначення: облік залишків товарів по партіях.

## Запуск
dotnet build
dotnet run --project src/Cli

## Середовище
.NET SDK 10.0, [Windows 10]

## Режими публікації
| Режим | RID | Розмір publish | Потрібен runtime |
| :--- | :--- | :--- | :--- |
| self-contained | win-x64 | ~70 МБ | ні |
| framework-dependent | win-x64 | ~0.2 МБ | так (.NET 10) |
| self-contained + SingleFile | win-x64 | ~70 МБ | ні |
| self-contained + Trimmed | win-x64 | ~19.3 | ні |

*Self-contained містить копію .NET runtime для конкретної RID, тому займає багато місця, але не вимагає встановленого .NET на машині користувача. Framework-dependent містить лише код застосунку, тому він малий, але потребує .NET 10.*