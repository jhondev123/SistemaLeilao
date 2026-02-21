using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Domain.Common
{

    public record SucessMessage(string Code, string Message)
    {
        public static readonly SucessMessage None = new(string.Empty, string.Empty);
    }
}
