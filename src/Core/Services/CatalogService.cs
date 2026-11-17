using Core.Abstractions;
using Core.Domain;
using Core.Events;

namespace Core.Services;

public sealed class CatalogService(ICatalogStore store, INotificationSink? sink = null)
{
    private readonly ICatalogStore _store = store ?? throw new ArgumentNullException(nameof(store));
    private readonly INotificationSink? _sink = sink;
    
    public event EventHandler<CatalogChangedEventArgs>? Changed;

    // ЄДИНЕ МІСЦЕ ПІДНЯТТЯ ПОДІЇ, ВИКЛИКАЄТЬСЯ ЛИШЕ ПІСЛЯ УСПІШНОЇ ЗМІНИ
     private void OnChanged(Product p, CatalogChangeKind kind, int delta, int? quantityAfter = null)
    {
        var args = new CatalogChangedEventArgs(p.Id, kind, delta, quantityAfter ?? p.Quantity);
        _sink?.Notify(args);
        Changed?.Invoke(this, args);
    }
    
    public Product Add(string sku, string name, string unit, int quantity)
    {
        var product = Product.Create(Guid.NewGuid().ToString("N")[..8], sku, name, unit, quantity);
        _store.Add(product);
        OnChanged(product, CatalogChangeKind.Added, quantity);
        return product;
    }

    public void Receive(string id, int quantity)
    {
        var product = _store.GetById(id)
            ?? throw new InvalidOperationException($"Немає запису з id={id}.");
        product.RegisterArrival(quantity);   // якщо тут виняток, до OnChanged не дійде
        _store.Update(product);
        OnChanged(product, CatalogChangeKind.Received, quantity);
    }

    public void Issue(string id, int quantity)
    {
        var product = _store.GetById(id)
            ?? throw new InvalidOperationException($"Немає запису з id={id}.");
        product.Issue(quantity);
        _store.Update(product);
        OnChanged(product, CatalogChangeKind.Issued, -quantity);
    }

    public bool Remove(string id)
    {
        var product = _store.GetById(id);
        if (product is null || !_store.Remove(id)) return false;
        OnChanged(product, CatalogChangeKind.Removed, -product.Quantity, quantityAfter: 0);
        return true;
    }

    public IReadOnlyList<Product> All() => _store.List();

    public Product? Find(string id) => _store.GetById(id);

    // КРИТЕРІЙ ЗАДАЄ ТОЙ, ХТО ВИКЛИКАЄ
    public IReadOnlyList<Product> FindAll(Func<Product, bool> predicate) =>
        _store.List().Where(predicate).ToList();
}