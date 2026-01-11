using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Domain.Interfaces
{
    public interface ITimestampEntity
    {
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
    }
}
