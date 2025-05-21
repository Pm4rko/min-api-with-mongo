using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class MongoMovieService : IMovieService
{
    private readonly IMongoCollection<Movie> _movies;

    public MongoMovieService(IOptions<DatabaseSettings> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        var database = client.GetDatabase("gbs");
        _movies = database.GetCollection<Movie>("movies");
    }

    public void Create(Movie movie)
    {
        _movies.InsertOne(movie);
    }

    public IEnumerable<Movie> Get()
    {
        return _movies.Find(_ => true).ToList();
    }

    public Movie Get(string id)
    {
        return _movies.Find(m => m.Id == id).FirstOrDefault();
    }

    public void Update(string id, Movie movie)
    {
        _movies.ReplaceOne(m => m.Id == id, movie);
    }

    public void Remove(string id)
    {
        _movies.DeleteOne(m => m.Id == id);
    }

    public string Check()
    {
        try
        {
            var dbs = _movies.Database.Client.ListDatabaseNames().ToList();
            return $"Zugriff auf MongoDB ok. Gefundene Datenbanken: {string.Join(", ", dbs)}";
        }
        catch (Exception ex)
        {
            return $"Fehler beim Zugriff auf MongoDB: {ex.Message}";
        }
    }
}