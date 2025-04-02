
namespace CSVProcessor.Models.DTO;

public class FilmResponseDTO
{
    public Guid Id { get; }
    public string Title { get; }
    public string Budget { get; }
    public string ReleaseDate { get; }
    public List<ActorResponseDTO> Actors { get; } = new List<ActorResponseDTO>();

    
    public FilmResponseDTO(FilmData data, bool includeActorsFilms = false)
    {
        Id = data.Id;
        Title = data.Title;
        Budget = data.Budget.ToString();
        ReleaseDate = data.ReleaseDate;
        foreach (var actor in data.Actors)
        {
            Actors.Add(new ActorResponseDTO(actor, includeActorsFilms));
        }
        
    }
    
}


