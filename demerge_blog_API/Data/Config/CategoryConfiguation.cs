using demerge_blog_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace demerge_blog_API.Data.Config
{
    public class CategoryConfiguation : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).HasColumnType("varchar").HasMaxLength(60).IsRequired();
            builder.HasIndex(p => p.Name).IsUnique(true);

            builder.ToTable("Categories");
        }
    }
}
