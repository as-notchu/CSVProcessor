using System.ComponentModel.DataAnnotations;

namespace CSVProcessor.Models;

public class FilmDTO
{
    [Required]
    public string Title { get; set; }
    [Required]
    public string Budget { get; set; }
    [Required]
    public string ReleaseDate { get; set; }
}