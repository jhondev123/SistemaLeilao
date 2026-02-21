using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Domain.Common
{

    public record SuccessMessage(string Code, string Message)
    {
        public static readonly SuccessMessage None = new(string.Empty, string.Empty);
        public override string ToString() => Message;
    }
}
