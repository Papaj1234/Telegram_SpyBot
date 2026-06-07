namespace TelegramSpyBot.Data.Context.Configurations;

public class UniverseConfiguration : IEntityTypeConfiguration<Universe>
{
    public void Configure(EntityTypeBuilder<Universe> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Description)
            .HasMaxLength(500);

        builder.Property(u => u.ImageUrl)
            .HasMaxLength(500);
    }
}