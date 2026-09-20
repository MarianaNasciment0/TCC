using Microsoft.Extensions.Logging;
using TCC.Helpers;

namespace TCC;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSans Regular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSans Semibold");
            });  

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Configuração do caminho do banco de dados SQLite para a Semana 2
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tcc_database.db3");
        builder.Services.AddSingleton<SQLiteDatabaseHelper>(s => new SQLiteDatabaseHelper(dbPath));

        return builder.Build();
    }
}