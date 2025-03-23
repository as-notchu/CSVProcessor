namespace CSVProcessor.Models;

public class FilmData
{
    public Guid Id { get; set; }
    
    public string Title { get; set; }
    
    public string Budget { get; set; }
    
    public string ReleaseDate { get; set; }

    public FilmData() { }
    
    public FilmData(FilmDTO data)
    {
        Id = Guid.NewGuid();
        Title = data.Title;
        Budget = data.Budget;
        ReleaseDate = data.ReleaseDate;
    }
    
}