namespace Core.Domain;

public enum MovementStatus { Draft, Confirmed, Cancelled }

public sealed class Movement
{
    public string Id { get; }
    public string Sku { get; }
    public int Amount { get; }
    public MovementStatus Status { get; private set; }

    private Movement(string id, string sku, int amount)
    {
        Id = id;
        Sku = sku;
        Amount = amount;
        Status = MovementStatus.Draft;
    }

    public static Movement Create(string id, string sku, int amount)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Ідентифікатор переміщення обов'язковий", nameof(id));
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), amount,
                "Кількість переміщення має бути більшою за нуль");

        return new Movement(id.Trim(), sku.Trim(), amount);
    }

    public void TransitionTo(MovementStatus target)
    {
        bool allowed = (Status, target) switch
        {
            (MovementStatus.Draft, MovementStatus.Confirmed) => true,
            (MovementStatus.Draft, MovementStatus.Cancelled) => true,
            _ => false
        };

        if (!allowed)
            throw new InvalidOperationException(
                $"Перехід {Status} → {target} неможливий для переміщення {Id}");

        Status = target;
    }
}