using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var movieDatabaseConfigSection = builder.Configuration.GetSection("DatabaseSettings");
builder.Services.Configure<DatabaseSettings>(movieDatabaseConfigSection);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/check", (Microsoft.Extensions.Options.IOptions<DatabaseSettings> options) =>
{
    try
    {
        var mongoDbConnectionString = options.Value.ConnectionString;

        var client = new MongoClient(mongoDbConnectionString);

        var databaseNames = client.ListDatabaseNames().ToList();

        var dbList = string.Join(", ", databaseNames);
        return $"Zugriff auf MongoDB ok. Gefundene Datenbanken: {dbList}";
    }
    catch (Exception ex)
    {
        return $"Fehler beim Zugriff auf MongoDB: {ex.Message}";
    }
});

app.Run();
