namespace CSVProcessor.Models;

public class Actor
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public List<FilmData> Films { get; set; } = new List<FilmData>();

    public Actor(string name)
    {
        Name = name;
        Id = Guid.NewGuid();
    }
}