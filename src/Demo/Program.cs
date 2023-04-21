using System;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Demo;

internal abstract class Program
{
    private static readonly LoggerConfiguration loggerBuilder = new LoggerConfiguration().WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                                                                                                           theme: AnsiConsoleTheme.Literate);

    private static void Main()
    {
        Log.Logger = loggerBuilder.CreateLogger();

        HmacHelper helper = new();
        Guid orderId = Guid.Parse("C6756D70-35D6-4D54-BFDE-C1FFB9F9719E");

        string url = $"https://bestellen.vandijk.nl/external/{orderId}";
        Log.Logger.Information("URL: {Url}", url);

        url = $"{url}?{HmacHelper.GenerateQueryString(orderId)}";

        Log.Logger.Information("URL Secure: {Url}", url);
        Log.Logger.Information("Validationresult : {Result}", helper.IsValidHash(url));
    }
}
