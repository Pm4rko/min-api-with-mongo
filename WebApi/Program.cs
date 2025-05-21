using MongoDB.Driver;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

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

var movies = new ConcurrentDictionary<string, Movie>();

// Insert Movie
app.MapPost("/api/movies", (Movie movie) =>
{
    if (movies.ContainsKey(movie.Id))
    {
        return Results.Conflict($"Movie mit Id {movie.Id} existiert bereits.");
    }

    movies[movie.Id] = movie;
    return Results.Ok(movie);
});     

// Get all Movies
app.MapGet("/api/movies", () =>
{
    return Results.Ok(movies.Values);
});

// Get Movie by Id
app.MapGet("/api/movies/{id}", (string id) =>
{
    if (movies.TryGetValue(id, out var movie))
    {
        return Results.Ok(movie);
    }

    return Results.NotFound();
});

// Update Movie
app.MapPut("/api/movies/{id}", (string id, Movie movie) =>
{
    if (!movies.ContainsKey(id))
    {
        return Results.NotFound();
    }

    movie.Id = id;
    movies[id] = movie;
    return Results.Ok(movie);
});

// Delete Movie
app.MapDelete("/api/movies/{id}", (string id) =>
{
    if (movies.TryRemove(id, out _))
    {
        return Results.Ok($"Movie mit Id {id} wurde gelöscht.");
    }

    return Results.NotFound();
});


app.Run();
