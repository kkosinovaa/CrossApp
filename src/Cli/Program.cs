using System.Runtime.InteropServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text;
Console.OutputEncoding = Encoding.UTF8;

var info = new
{
    Title = "CrossApp — практикум з крос-платформного програмування",
    Student = "Косінова Катерина, ФЕІ-35",
    OsDescription = RuntimeInformation.OSDescription,
    OsEnvironment = Environment.OSVersion.ToString(),
    ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
    ClrVersion = Environment.Version.ToString(),
    Runtime = RuntimeInformation.FrameworkDescription,
    AppDirectory = AppContext.BaseDirectory,
    CurrentDirectory = Environment.CurrentDirectory,
    Domain = "Замовлення (клієнти, товари, замовлення, рядки замовлень)"
};

if (args.Contains("--json"))
{
    var options = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    Console.WriteLine(JsonSerializer.Serialize(info, options));
    return;
}

Console.WriteLine(info.Title);
Console.WriteLine($"Студент: {info.Student}");
Console.WriteLine(new string('-', 52));
Console.WriteLine($"ОС (OSDescription) : {info.OsDescription}");
Console.WriteLine($"ОС (Environment)   : {info.OsEnvironment}");
Console.WriteLine($"Архітектура процесу: {info.ProcessArchitecture}");
Console.WriteLine($"Версія .NET (CLR)  : {info.ClrVersion}");
Console.WriteLine($"Runtime            : {info.Runtime}");
Console.WriteLine($"Каталог застосунку : {info.AppDirectory}");
Console.WriteLine($"Поточний каталог   : {info.CurrentDirectory}");
Console.WriteLine(new string('-', 52));
Console.WriteLine($"Предметна область  : {info.Domain}");