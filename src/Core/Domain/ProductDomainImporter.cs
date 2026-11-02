using Core.Dto;
using Core.Import;

namespace Core.Domain;

public static class ProductDomainImporter
{
    public static (IReadOnlyList<Product> Products, IReadOnlyList<string> Errors) LoadValidated(string path)
    {
        ImportResult<ProductDto> raw = ProductCsvImporter.Load(path);

        var products = new List<Product>();
        var errors = new List<string>(raw.Errors);

        foreach (ProductDto dto in raw.Items)
        {
            try
            {
                products.Add(Product.FromDto(dto));
            }
            catch (Exception ex)
            {
                errors.Add($"{dto.Id}: порушено інваріант домену — {ex.Message}");
            }
        }

        return (products, errors);
    }
}