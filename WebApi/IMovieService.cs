public interface IMovieService
{
    string Check();
    void Create(Movie movie);
    IEnumerable<Movie> Get();
    Movie Get(string id);
    void Update(string id, Movie movie);
    void Remove(string id);
}