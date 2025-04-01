using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CSVProcessor.Helpers;

namespace CSVProcessor.Models;

public class FilmRequestDTO
{
    [Required]
    public string Title { get; set; }
    [Required]
    public long Budget { get; set; }
    [Required]
    public string ReleaseDate { get; set; }

    [Required]
    public List<string> Actors { get; set; } = new List<string>();
}