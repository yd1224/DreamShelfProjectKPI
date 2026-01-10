using DreamShelf.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DreamShelf.API.Configurations.Entities;

public class BookImageConfiguration: IEntityTypeConfiguration<BookImage>
{
    public void Configure(EntityTypeBuilder<BookImage> builder)
    {
        builder.HasKey(x => new {x.BookId, x.ImageId});
        builder.HasIndex(x => x.ChapterId);
    }
}