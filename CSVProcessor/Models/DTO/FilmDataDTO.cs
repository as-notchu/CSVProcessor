using CSVProcessor.Interfaces;

namespace CSVProcessor.Models.DTO;

public class FilmDataDTO
{
    public Guid Id { get; }
    public string Title { get; }
    public string Budget { get; }
    public string ReleaseDate { get; }
    public List<ActorDataDTO> Actors { get; } = new List<ActorDataDTO>();

    public FilmDataDTO(FilmData data)
    {
        Id = data.Id;
        Title = data.Title;
        Budget = data.Budget;
        ReleaseDate = data.ReleaseDate;
        foreach (var actor in data.Actors)
        {
            Actors.Add(new ActorDataDTO()
            {
                Id = actor.Id,
                Name = actor.Name,
            });
        }
    }
    
}

public class ActorDataDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
