using MongoDB.Driver;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var movieDatabaseConfigSection = builder.Configuration.GetSection("DatabaseSettings");
builder.Services.Configure<DatabaseSettings>(movieDatabaseConfigSection);
builder.Services.AddSingleton<IMovieService, MongoMovieService>();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/check", (IMovieService movieService) => {
    return movieService.Check();
});

app.MapPost("/api/movies", (IMovieService service, Movie movie) =>
{
    try
    {
        service.Create(movie);
        return Results.Ok(movie);
    }
    catch (Exception ex)
    {
        return Results.Conflict(ex.Message);
    }
});

app.MapGet("/api/movies", (IMovieService service) =>
{
    return Results.Ok(service.Get());
});

app.MapGet("/api/movies/{id}", (IMovieService service, string id) =>
{
    var movie = service.Get(id);
    return movie is null ? Results.NotFound() : Results.Ok(movie);
});

app.MapPut("/api/movies/{id}", (IMovieService service, string id, Movie movie) =>
{
    var existing = service.Get(id);
    if (existing is null) return Results.NotFound();

    movie.Id = id;
    service.Update(id, movie);
    return Results.Ok(movie);
});

app.MapDelete("/api/movies/{id}", (IMovieService service, string id) =>
{
    var existing = service.Get(id);
    if (existing is null) return Results.NotFound();

    service.Remove(id);
    return Results.Ok($"Movie mit Id {id} wurde gelöscht.");
});

app.Run();