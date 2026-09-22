using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Runtime.InteropServices;
using System.Text.Json;

bool jsonOutput = args.Contains("--json");

var info = new
{
    Title = "CrossApp – практикум з крос-платформного програмування",
    Student = "Булгакова Діана, група ФЕІ-31",
    OSDescription = RuntimeInformation.OSDescription,
    OSVersion = Environment.OSVersion.ToString(),
    ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
    DotnetVersion = Environment.Version.ToString(),
    Runtime = RuntimeInformation.FrameworkDescription,
    AppDirectory = AppContext.BaseDirectory,
    CurrentDirectory = Environment.CurrentDirectory,
    Domain = "Склад (товари, партії, залишки, переміщення)"
};

if (jsonOutput)
{
    var options = new JsonSerializerOptions
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
    };
    string json = JsonSerializer.Serialize(info, options);
    Console.WriteLine(json);
}
else
{
    Console.WriteLine(info.Title);
    Console.WriteLine($"Студент: {info.Student}");
    Console.WriteLine(new string('-', 52));
    Console.WriteLine($"ОС (OSDescription) : {info.OSDescription}");
    Console.WriteLine($"ОС (Environment) : {info.OSVersion}");
    Console.WriteLine($"Архітектура процесу : {info.ProcessArchitecture}");
    Console.WriteLine($"Версія .NET (CLR) : {info.DotnetVersion}");
    Console.WriteLine($"Runtime : {info.Runtime}");
    Console.WriteLine($"Каталог застосунку : {info.AppDirectory}");
    Console.WriteLine($"Поточний каталог : {info.CurrentDirectory}");
    Console.WriteLine(new string('-', 52));
    Console.WriteLine($"Предметна область: {info.Domain}");
}

