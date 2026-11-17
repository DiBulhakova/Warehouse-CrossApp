using Core.Abstractions;
using Core.Events;

internal sealed class ConsoleSink : INotificationSink
{
    public void Notify(CatalogChangedEventArgs e) =>
        Console.WriteLine($"[sink] {e.Kind} {e.Id} {e.Delta:+0;-0} → {e.QuantityAfter}");
}

internal sealed class FileSink(string path) : INotificationSink
{
    public void Notify(CatalogChangedEventArgs e)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.AppendAllText(path, $"{e.At:HH:mm:ss} {e.Kind} {e.Id} {e.Delta:+0;-0}{Environment.NewLine}");
    }
}