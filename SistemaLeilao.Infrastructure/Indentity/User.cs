using Microsoft.AspNetCore.Identity;
using SistemaLeilao.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Infrastructure.Indentity
{
    public class User : IdentityUser<long>, ITimestampEntity, ISoftDeletable
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid ExternalId { get; set; } = Guid.NewGuid();

    }
}
