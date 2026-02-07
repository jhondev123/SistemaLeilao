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
        public decimal CurrentPrice { get; private set; }
        public decimal MinimumIncrement { get; set; }

        // Vencedor
        public long? BidderWinnerId { get; private set; }
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

        public (bool success, string errorMessage) CanPlaceBid(decimal bidAmount)
        {
            if (Status != AuctionStatus.OPEN)
            {
                return (false, "O leilão não está aberto para lances.");
            }

            if (DateTime.UtcNow > EndDate)
            {
                return (false, "O leilão já terminou.");
            }
            if(bidAmount < (CurrentPrice + MinimumIncrement))
            {
                return (false, $"O lance deve ser pelo menos {MinimumIncrement:C} maior que o preço atual.");
            }
            return (true, string.Empty);
        }

        public (bool success, string errorMessage) ApplyNewBid(decimal newPrice, long bidderId)
        {
            var (canPlace, error) = CanPlaceBid(newPrice);

            if (!canPlace)
            {
                return (false, error);
            }
            CurrentPrice = newPrice;
            BidderWinnerId = bidderId;

            return (true, string.Empty);
        }
    }
}