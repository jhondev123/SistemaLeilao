using SistemaLeilao.Core.Domain.Entities.Common;
using SistemaLeilao.Core.Domain.Enums;

namespace SistemaLeilao.Core.Domain.Entities
{
    public class Auction : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public AuctionStatus Status { get; set; }
        public byte[]? Image { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal StartingPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal MinimumIncrement { get; set; }

        // Vencedor
        public long? BidderWinnerId { get; set; }
        public Bidder? BidderWinner { get; set; }

        // Criador
        public long AuctioneerId { get; set; }
        public Auctioneer Auctioneer { get; set; } = null!;

        public List<Bid> Bids { get; set; } = new();

        public Auction() { }
        public Auction(
            string title,
            long auctioneerId, 
            decimal startingPrice, 
            decimal currentPrice, 
            DateTime startDate, 
            DateTime endDate, 
            string? description, 
            byte[]? image, 
            decimal minimumIncrement, 
            AuctionStatus status)
        {
            Title = title;
            AuctioneerId = auctioneerId;
            StartingPrice = startingPrice;
            CurrentPrice = currentPrice;
            StartDate = startDate;
            EndDate = endDate;
            Description = description;
            Image = image;
            MinimumIncrement = minimumIncrement;
            Status = status;
        }

        public bool CanPlaceBid(decimal bidAmount)
        {
            if (Status != AuctionStatus.OPEN) return false;
            if (DateTime.UtcNow > EndDate) return false;

            // O lance deve ser maior que o preço atual + o incremento mínimo
            return bidAmount >= (CurrentPrice + MinimumIncrement);
        }

        public void UpdatePrice(decimal newPrice, long bidderId)
        {
            if (CanPlaceBid(newPrice))
            {
                CurrentPrice = newPrice;
                BidderWinnerId = bidderId;
            }
        }
    }
}