namespace TelegramSpyBot.Data.Context.Configurations;

public class CharacterConfiguration : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.ImageUrl)
            .HasMaxLength(500);

        builder.HasOne(c => c.Universe)
            .WithMany(u => u.Characters)
            .HasForeignKey(c => c.UniverseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}