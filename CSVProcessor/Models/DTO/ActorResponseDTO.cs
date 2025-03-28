namespace CSVProcessor.Models.DTO;

public class ActorResponseDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    public Dictionary<string, Guid> Films { get; set; }
    
    public List<string> FilmNames { get; set; }


    public ActorResponseDTO(Actor actor,  bool includeFilm = false)
    {
        Id = actor.Id;
        Name = actor.Name;
        Films = new Dictionary<string, Guid>();
        if (includeFilm)
        {
            foreach (var film in actor.Films)
            {
                Films[film.Title] = film.Id;
            }
        }
        
    }
}