using demerge_blog_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace demerge_blog_API.Data.Config
{
    public class CommentViewConfiguration : IEntityTypeConfiguration<CommentView>
    {
        public void Configure(EntityTypeBuilder<CommentView> builder)
        {
            builder.ToView("GetAllComments").HasNoKey();
        }
    }
    public class CommentDetailsConfiguration : IEntityTypeConfiguration<CommentDetails>
    {
        public void Configure(EntityTypeBuilder<CommentDetails> builder)
        {
            builder.ToFunction("GetCommentDetails").HasNoKey();
        }
    }
}
