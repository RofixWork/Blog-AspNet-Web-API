using demerge_blog_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace demerge_blog_API.Data.Config
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Title).HasColumnType("varchar").HasMaxLength(150).IsRequired();
            builder.Property(p => p.Content).HasColumnType("text").IsRequired();
            builder.Property(p => p.Image).HasColumnType("image").IsRequired();
            builder.Property(p => p.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("getdate()");
            builder.Property(p => p.UpdateAt).HasColumnType("datetime2").HasDefaultValueSql("getdate()");

            builder.HasOne(p => p.Category).WithMany(p => p.Posts).HasForeignKey(p => p.CategoryId).IsRequired();

            builder.HasOne(p => p.User).WithMany(p => p.Posts).HasForeignKey(p => p.UserId).IsRequired();

            builder.ToTable("Posts");
        }
    }
}
