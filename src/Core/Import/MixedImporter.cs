using Core.Dto;

namespace Core.Import;

public sealed record MixedImportResult(
    IReadOnlyList<ProductDto> Products,
    IReadOnlyList<WarehouseDto> Warehouses,
    IReadOnlyList<string> Errors);

public static class MixedImporter
{
    private const char Separator = ';';

    public static MixedImportResult Load(string path)
    {
        var products = new List<ProductDto>();
        var warehouses = new List<WarehouseDto>();
        var errors = new List<string>();

        string[] lines = File.ReadAllLines(path);

        for (int i = 0; i < lines.Length; i++)
        {
            int number = i + 1;
            string line = lines[i];

            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] parts = line.Split(Separator, StringSplitOptions.TrimEntries);

            switch (parts)
            {
                case ["P", var id, var sku, var name, var unit, var qty]
                    when int.TryParse(qty, out int q) && q >= 0:
                    products.Add(new ProductDto(id, sku, name, unit, q));
                    break;

                case ["W", var id, var name, var location]:
                    warehouses.Add(new WarehouseDto(id, name, location));
                    break;

                default:
                    errors.Add($"рядок {number}: не вдалося розпізнати тип запису");
                    break;
            }
        }

        return new MixedImportResult(products, warehouses, errors);
    }
}