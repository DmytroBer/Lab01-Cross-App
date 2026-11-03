using Core.Dto;

namespace Core.Import;

public static class MixedCsvImporter
{
    private const char Separator = ';';

    public static MixedImportResult Load(string path)
    {
        var products = new List<ProductDto>();
        var warehouses = new List<WarehouseDto>();
        var errors = new List<string>();
        string[] lines = File.ReadAllLines(path);
        int totalLines = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            int number = i + 1;
            string line = lines[i];

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            if (number == 1 && line.StartsWith("id", StringComparison.OrdinalIgnoreCase))
                continue;

            totalLines++; // Рахуємо тільки рядки з даними

            switch (ParseLine(line))
            {
                case ParseProductOk p: products.Add(p.Value); break;
                case ParseWarehouseOk w: warehouses.Add(w.Value); break;
                case ParseFailed failed: errors.Add($"рядок {number}: {failed.Reason}"); break;
            }
        }
        return new MixedImportResult(products, warehouses, errors, totalLines);
    }

    private static ParseOutcome ParseLine(string line)
    {
        string[] parts = line.Split(Separator, StringSplitOptions.TrimEntries);
        return parts switch
        {
            // Патерни для товарів (P-...)
            [var id, _, _, _, _] when id.StartsWith("P") && parts.Length < 5 
                => new ParseFailed($"очікую 5 колонок, отримав {parts.Length}"),
            [var id, "", "", ..] when id.StartsWith("P") 
                => new ParseFailed("SKU або назва порожні"),
            [var id, _, _, _, var qty] when id.StartsWith("P") && (!int.TryParse(qty, out int q) || q < 0)
                => new ParseFailed($"кількість '{qty}' не є невід'ємним числом"),
            [var id, var sku, var name, var unit, var qty] when id.StartsWith("P")
                => new ParseProductOk(new ProductDto(id, sku, name, unit, int.Parse(qty))),

            // Патерни для складів (W-...)
            [var id, var name, var loc] when id.StartsWith("W")
                => new ParseWarehouseOk(new WarehouseDto(id, name, loc)),
            [var id, ..] when id.StartsWith("W")
                => new ParseFailed($"очікую 3 колонки для складу, отримав {parts.Length}"),

            _ => new ParseFailed($"невідомий тип запису: {parts[0]}")
        };
    }

    private abstract record ParseOutcome;
    private sealed record ParseProductOk(ProductDto Value) : ParseOutcome;
    private sealed record ParseWarehouseOk(WarehouseDto Value) : ParseOutcome;
    private sealed record ParseFailed(string Reason) : ParseOutcome;
}