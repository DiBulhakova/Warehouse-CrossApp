using Core.Abstractions;
using Core.Domain;

namespace Core.Storage;

public sealed class CachingCatalogStore(ICatalogStore inner, Action<string>? log = null) : ICatalogStore
{
    private readonly ICatalogStore _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    private IReadOnlyList<Product>? _list;

    public IReadOnlyList<Product> List()
    {
        if (_list is null)
        {
            log?.Invoke("  кеш: промах, читаю з внутрішнього сховища");
            _list = _inner.List();
        }
        else
        {
            log?.Invoke("  кеш: влучання");
        }
        return _list;
    }

    public Product? GetById(string id) => _inner.GetById(id);

    public void Add(Product item)
    {
        _inner.Add(item);
        _list = null;
    }

    public void Update(Product item)
    {
        _inner.Update(item);
        _list = null;
    }

    public bool Remove(string id)
    {
        bool removed = _inner.Remove(id);
        if (removed) _list = null;
        return removed;
    }
}