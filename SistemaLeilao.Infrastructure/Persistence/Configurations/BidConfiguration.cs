using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaLeilao.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Infrastructure.Persistence.Configurations
{
    public class BidConfiguration : IEntityTypeConfiguration<Bid>
    {
        public void Configure(EntityTypeBuilder<Bid> builder)
        {
            builder.ToTable("Bids");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Version)
                .IsRowVersion();

            builder.Property(e => e.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(e => e.BidDate)
                .IsRequired();

            builder.HasOne(e => e.Auction)
                .WithMany(a => a.Bids)
                .HasForeignKey(e => e.AuctionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Bidder)
                .WithMany(b => b.Bids)
                .HasForeignKey(e => e.BidderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
