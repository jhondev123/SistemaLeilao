using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Infrastructure.Indentity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Infrastructure.Persistence.Configurations
{
    public class BidderConfiguration : IEntityTypeConfiguration<Bidder>
    {
        public void Configure(EntityTypeBuilder<Bidder> builder)
        {
            builder.ToTable("Bidders");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Version)
                .IsRowVersion();

            builder.Property(e => e.PerfilName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.PhoneNumber)
                .HasMaxLength(15);

            builder.Property(e => e.WalletBalance)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.HasMany(e => e.Bids)
                .WithOne(e => e.Bidder)
                .HasForeignKey(e => e.BidderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
