using SistemaLeilao.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Domain.Entities.Common
{
    public abstract class BaseEntity : ITimestampEntity, ISoftDeletable
    {
        public long Id { get; set; }
        public Guid ExternalId { get; set; } = Guid.NewGuid();
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public uint Version { get; set; }
    }
}
