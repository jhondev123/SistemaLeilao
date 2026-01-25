using Microsoft.AspNetCore.Identity;
using SistemaLeilao.Core.Application.Interfaces;
using SistemaLeilao.Infrastructure.Indentity;
using System.Security.Claims;

namespace SistemaLeilao.Infrastructure.Services.Auth
{
    public class AuthService : IAuthService
    {
        private UserManager<User> _userManager { get; set; }
        private SignInManager<User> _signInManager { get; set; }
        private IJwtTokenGeneratorService tokenService { get; set; }
        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IJwtTokenGeneratorService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.tokenService = tokenService;
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(string name, string email, string password)
        {
            var user = new User { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description));

            await _userManager.AddClaimAsync(user, new Claim("FullName", name));
            return (true, Enumerable.Empty<string>());
        }

        public async Task<(bool Succeeded, string? Token)> LoginAsync(string email, string password)
        {
            User? user = await _userManager.FindByEmailAsync(email);
            if (user == null) return (false, null);

            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded) return (false, null);

            var roles = await _userManager.GetRolesAsync(user);

            var token = tokenService.GenerateToken(user.Id.ToString(), user.Email ?? "", user.UserName ?? "", roles);
            return (true, token);
        }

    }
}
