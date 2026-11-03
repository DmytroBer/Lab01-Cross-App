using Core.Dto;
using Core.Import;

string path = args.Length > 0 ? args[0] : Path.Combine("data", "sample.csv");
if (!File.Exists(path))
{
    Console.WriteLine($"Файл не знайдено: {Path.GetFullPath(path)}");
    return 1;
}

// Завдання 1: Вибір імпортера за розширенням файлу
MixedImportResult result = Path.GetExtension(path).ToLower() switch
{
    ".csv" => MixedCsvImporter.Load(path),
    ".json" => MixedJsonImporter.Load(path),
    _ => throw new NotSupportedException("Формат файлу не підтримується. Використовуйте .csv або .json")
};

Console.WriteLine($"--- ТОВАРИ ({result.Products.Count}) ---");
foreach (ProductDto p in result.Products.Take(3))
    Console.WriteLine($" {p.Id,-6} {p.Sku,-10} {p.Name,-26} {p.Quantity, 5} {p.Unit}");

Console.WriteLine($"\n--- СКЛАДИ ({result.Warehouses.Count}) ---");
foreach (WarehouseDto w in result.Warehouses.Take(3))
    Console.WriteLine($" {w.Id,-6} {w.Name,-20} {w.Location}");

if (result.Errors.Count > 0)
{
    Console.WriteLine($"\n--- ПОМИЛКИ ({result.Errors.Count}) ---");
    foreach (string e in result.Errors)
        Console.WriteLine($" ! {e}");
}

// Завдання 3: Статистика імпорту одним рядком
int total = result.TotalLines;
int accepted = result.Products.Count + result.Warehouses.Count;
int skipped = result.Errors.Count;
double errorRate = total > 0 ? (double)skipped / total * 100 : 0;

Console.WriteLine($"\n[Статистика]: усього {total} | прийнято {accepted} | пропущено {skipped} | помилок {errorRate:F1}%");

return result.Errors.Count == 0 ? 0 : 1;