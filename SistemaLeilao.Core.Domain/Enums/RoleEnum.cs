using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SistemaLeilao.Core.Domain.Enums
{
    public enum RoleEnum
    {
        [Description("Bidder")]
        Bidder,
        [Description("Auctioneer")]
        Auctioneer,
        [Description("Admin")]
        Admin
    }
}
