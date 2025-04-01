namespace CSVProcessor.Models;

public class FilmData
{
    public Guid Id { get; set; }
    
    public string Title { get; set; }
    
    public long Budget { get; set; }
    
    public string ReleaseDate { get; set; }
    
    public List<Actor> Actors { get; set; } = new List<Actor>();

    public FilmData() { }
    
    public FilmData(FilmCreateDTO data)
    {
        Id = Guid.NewGuid();
        Title = data.Title;
        Budget = data.Budget;
        ReleaseDate = data.ReleaseDate;
    }
    
}