namespace TelegramSpyBot.Data.Models;

public class Player : BaseEntity
{
    [Required]
    public int SessionId { get; set; }

    [Required]
    public long TelegramUserId { get; set; }

    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public bool IsSpy { get; set; }

    public GameSession Session { get; set; } = null!;
}