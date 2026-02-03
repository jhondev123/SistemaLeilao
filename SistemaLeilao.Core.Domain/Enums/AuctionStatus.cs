using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Domain.Enums
{
    public enum AuctionStatus
    {
        AWAITING_START = 1,
        OPEN = 2,
        CLOSED = 3,
        AWAITING_PAYMENT = 4,
    }
}
