using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CSVProcessor.Helpers;

namespace CSVProcessor.Models;

public class FilmCreateDTO
{
    [Required]
    public string Title { get; set; }
    [Required]
    public string Budget { get; set; }
    [Required]
    public string ReleaseDate { get; set; }

    [Required]
    [TypeConverter(typeof(ActorListConverter))]
    public List<string> Actors { get; set; } = new List<string>();
}