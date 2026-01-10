using DreamShelf.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DreamShelf.API.Configurations.Entities;

public class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
{
    public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder.HasIndex(urt => urt.Token).IsUnique();
        builder.HasIndex(urt => urt.DeviceInfo);
    }
}