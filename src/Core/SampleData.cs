using Core.Domain;

namespace Core;

public static class SampleData
{
    public static IEnumerable<Product> Products() =>
    [
        Product.Create("P-001", "SKU-001", "Цемент М400 25кг", "шт", 120),
        Product.Create("P-002", "SKU-002", "Пісок річковий", "т", 40),
        Product.Create("P-003", "SKU-003", "Цегла червона", "шт", 5000),
        Product.Create("P-004", "SKU-004", "Щебінь фракція 5-20", "т", 65),
        Product.Create("P-005", "SKU-005", "Арматура 12мм", "м", 800),
        Product.Create("P-006", "SKU-006", "Гіпсокартон 12.5мм", "шт", 150),
        Product.Create("P-007", "SKU-007", "Профіль CD 60", "шт", 300),
        Product.Create("P-008", "SKU-008", "Шпаклівка фінішна 25кг", "шт", 45),
        Product.Create("P-009", "SKU-009", "Фарба біла 10л", "шт", 30),
        Product.Create("P-010", "SKU-010", "Грунтовка 5л", "шт", 55),
        Product.Create("P-011", "SKU-011", "Кабель ВВГ 3x2.5", "м", 1200),
        Product.Create("P-012", "SKU-012", "Труба ПП 20мм", "м", 400),
        Product.Create("P-013", "SKU-013", "Утеплювач 100мм", "уп", 25),
        Product.Create("P-014", "SKU-014", "Плитковий клей 25кг", "шт", 90),
        Product.Create("P-015", "SKU-015", "Саморізи 4.2x32", "уп", 200),
        Product.Create("P-016", "SKU-016", "Пінополіуретанова піна", "шт", 70),
    ];
}