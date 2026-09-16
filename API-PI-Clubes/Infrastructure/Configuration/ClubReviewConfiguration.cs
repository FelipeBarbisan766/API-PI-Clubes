using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_PI_Clubes.Infrastructure.Configurations
{
    public class ClubReviewConfiguration : IEntityTypeConfiguration<ClubReview>
    {
        public void Configure(EntityTypeBuilder<ClubReview> builder)
        {
            builder.ToTable("ClubReviews");

            builder.Property(cr => cr.Rating)
                .HasColumnType("decimal(2,1)")
                .IsRequired();

            builder.HasIndex(cr => new { cr.ClubId, cr.PlayerId })
                .IsUnique();

            builder.HasOne(cr => cr.Club)
                .WithMany(c => c.Reviews)
                .HasForeignKey(cr => cr.ClubId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cr => cr.Player)
                .WithMany()
                .HasForeignKey(cr => cr.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}