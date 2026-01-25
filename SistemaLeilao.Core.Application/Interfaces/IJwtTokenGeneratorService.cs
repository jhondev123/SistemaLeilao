using System;
using System.Collections.Generic;
using System.Text;
namespace SistemaLeilao.Core.Application.Interfaces
{
    public interface IJwtTokenGeneratorService
    {
        string GenerateToken(string userId, string email,string userName, IList<string> roles);
    }
}
