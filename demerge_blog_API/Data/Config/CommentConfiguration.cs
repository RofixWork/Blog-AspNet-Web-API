using demerge_blog_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace demerge_blog_API.Data.Config
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.CommentText).HasColumnType("text").IsRequired();
            builder.Property(p => p.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("getdate()");
            builder.Property(p => p.UpdatedAt).HasColumnType("datetime2").HasDefaultValueSql("getdate()");

            builder.HasOne(p => p.Post).WithMany(p => p.Comments).HasForeignKey(p => p.PostId).IsRequired();

            builder.HasOne(p => p.User).WithMany(p => p.Comments).HasForeignKey(p => p.UserId).IsRequired();

            builder.ToTable("Comments");
        }
    }
}
