using System.Text.Json;
using Core.Dto;

namespace Core.Import;

public static class MixedJsonImporter
{
    public static MixedImportResult Load(string path)
    {
        string json = File.ReadAllText(path);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        
        try 
        {
            var data = JsonSerializer.Deserialize<JsonData>(json, options) ?? new JsonData([], []);
            int total = data.Products.Count + data.Warehouses.Count;
            return new MixedImportResult(data.Products, data.Warehouses, [], total);
        }
        catch (Exception ex) 
        {
            return new MixedImportResult([], [], [$"Помилка JSON: {ex.Message}"], 1);
        }
    }
    
    // Допоміжний record для мапінгу JSON структури
    private record JsonData(List<ProductDto> Products, List<WarehouseDto> Warehouses);
}