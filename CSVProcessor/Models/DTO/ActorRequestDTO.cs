using System.ComponentModel.DataAnnotations;
using CSVProcessor.Enum;
namespace CSVProcessor.Models.DTO;

public class ActorRequestDTO
{
    public Guid? Id { get; set; }
    [Required]
    public string Name { get; set; }
    
    public List<string>? FilmNames { get; set; }

    
}