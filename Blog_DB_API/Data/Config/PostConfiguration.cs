using Blog_DB_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog_DB_API.Data.Config
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Title).HasColumnType("varchar").HasMaxLength(255).IsRequired();
            builder.Property(p => p.Content).HasColumnType("text").IsRequired();
            builder.Property(p => p.PublicationDate).HasColumnType("datetime2").HasDefaultValueSql("getdate()");
            builder.Property(p => p.Image).HasColumnType("image").IsRequired();

            builder.ToTable("Posts");
        }
    }
}
