using SistemaLeilao.Core.Domain.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Domain.Common
{
    public record ErrorMessage(string Code, string Message)
    {
        public static readonly ErrorMessage None = new(string.Empty, string.Empty);
        public override string ToString() => Message;
    }
}
