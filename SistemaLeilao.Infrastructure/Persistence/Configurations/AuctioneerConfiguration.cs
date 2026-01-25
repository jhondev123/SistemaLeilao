using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaLeilao.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Infrastructure.Persistence.Configurations
{
    public class AuctioneerConfiguration : IEntityTypeConfiguration<Auctioneer>
    {
        public void Configure(EntityTypeBuilder<Auctioneer> builder)
        {

            builder.Property(a => a.Bio)
                .HasMaxLength(1000);

            builder.Property(a => a.Rating)
                .HasDefaultValue(5.0);

            builder.HasMany(a => a.Auctions)
                .WithOne(au => au.Auctioneer)
                .HasForeignKey(au => au.AuctioneerId);
        }
    }
}
