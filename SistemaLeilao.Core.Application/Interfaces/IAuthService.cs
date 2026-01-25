using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(string name, string email, string password);
        Task<(bool Succeeded, string? Token)> LoginAsync(string email, string password);
    }
}
