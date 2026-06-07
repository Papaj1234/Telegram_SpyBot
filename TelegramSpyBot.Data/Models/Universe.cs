namespace TelegramSpyBot.Data.Models;

public class Universe : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public ICollection<Character> Characters { get; set; } = new List<Character>();
}