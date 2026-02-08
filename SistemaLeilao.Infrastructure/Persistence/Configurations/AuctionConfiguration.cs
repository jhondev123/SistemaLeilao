using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaLeilao.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Infrastructure.Persistence.Configurations
{
    public class AuctionConfiguration : IEntityTypeConfiguration<Auction>
    {
        public void Configure(EntityTypeBuilder<Auction> builder)
        {
            builder.ToTable("Auctions");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Version)
                .IsRowVersion();

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Description)
                .HasMaxLength(1000);

            builder.Property(e => e.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(e => e.Image)
                .IsRequired(false);

            builder.Property(e => e.StartDate)
                .IsRequired();

            builder.Property(e => e.EndDate)
                .IsRequired();

            builder.Property(e => e.StartingPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(e => e.CurrentPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(e => e.MinimumIncrement)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.HasOne(e => e.BidderWinner)
                    .WithMany()
                    .HasForeignKey(e => e.BidderWinnerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

            builder.HasOne(e => e.Auctioneer)
                .WithMany()
                .HasForeignKey(e => e.AuctioneerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.HasMany(e => e.Bids)
                .WithOne(e => e.Auction)
                .HasForeignKey(e => e.AuctionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
