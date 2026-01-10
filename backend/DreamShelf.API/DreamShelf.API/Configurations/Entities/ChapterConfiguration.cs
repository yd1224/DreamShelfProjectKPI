using DreamShelf.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DreamShelf.API.Configurations.Entities;

public class ChapterConfiguration : IEntityTypeConfiguration<Chapter>
{
    public void Configure(EntityTypeBuilder<Chapter> builder)
    {
        builder.HasIndex(c => new {c.ChapterNumber, c.BookId}).IsUnique();

        builder.HasMany(c => c.BookImages)
            .WithOne(bi => bi.Chapter)
            .HasForeignKey(bi => bi.ChapterId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}