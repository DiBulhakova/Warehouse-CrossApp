namespace Core.Domain;

public sealed class Warehouse
{
    public string Id { get; }
    public string Name { get; }
    public bool IsClosed { get; private set; }

    private Warehouse(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public static Warehouse Create(string id, string name)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Ідентифікатор складу обов'язковий", nameof(id));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Назва складу не може бути порожньою", nameof(name));

        return new Warehouse(id.Trim(), name.Trim());
    }

    public void Close() => IsClosed = true;
}