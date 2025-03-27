namespace CSVProcessor.Models.DTO;

public class ActorRequestDTO
{
    public Guid? Id { get; set; } = null;
    public string Name { get; set; }
    public List<string> FilmNames { get; set; } = new();
    
}