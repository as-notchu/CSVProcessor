namespace CSVProcessor.Models.DTO;

public class ActorRequestDTO
{
    public string Name { get; set; }
    
    public List<string> FilmNames { get; set; } = new List<string>();
    
}