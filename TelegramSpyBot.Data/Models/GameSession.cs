namespace TelegramSpyBot.Data.Models;

public class GameSession : BaseEntity
{
    [Required]
    public long ChatId { get; set; }

    [Required]
    public int CharacterId { get; set; }

    [Required]
    public GameStatus Status { get; set; } = GameStatus.Waiting;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FinishedAt { get; set; }

    public Character Character { get; set; } = null!;
    public ICollection<Player> Players { get; set; } = new List<Player>();
}