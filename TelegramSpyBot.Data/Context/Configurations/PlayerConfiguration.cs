namespace TelegramSpyBot.Data.Context.Configurations;

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.TelegramUserId)
            .IsRequired();

        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Username)
            .HasMaxLength(100);

        builder.Property(p => p.IsSpy)
            .IsRequired();

        builder.HasOne(p => p.Session)
            .WithMany(s => s.Players)
            .HasForeignKey(p => p.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}