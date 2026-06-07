namespace TelegramSpyBot.Data.Context.Configurations;

public class GameSessionConfiguration : IEntityTypeConfiguration<GameSession>
{
    public void Configure(EntityTypeBuilder<GameSession> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.ChatId)
            .IsRequired();

        builder.Property(g => g.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(g => g.CreatedAt)
            .IsRequired();

        builder.HasOne(g => g.Character)
            .WithMany()
            .HasForeignKey(g => g.CharacterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}