using System;
using System.Collections.Generic;
using System.Text;
namespace SistemaLeilao.Core.Application.Interfaces
{
    public interface IJwtTokenGeneratorService
    {
        string GenerateToken(Guid userExternalId, string email, string userName, IList<string> roles);
    }
}
