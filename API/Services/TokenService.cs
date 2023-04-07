using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        // There are both symmetricsecurity key and asymetric key
        // SymmetricSecurityKey - same key is used to encrypt the data and same key is used to decrypt the data
        // Asymetric key - private key on server encrypts data and public key on client decrypts the data
        private readonly SymmetricSecurityKey _key;      
        private readonly UserManager<AppUser> _userManager;
        public TokenService(IConfiguration config, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public async Task<string> CreateToken(AppUser user)
        {
            /*A claim is something, a bit of information that a user claims and in this case we're going to set
              our claims to the user name.*/ 
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),                                               
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),                                               
            };

            var roles = await _userManager.GetRolesAsync(user);

            claims.AddRange(roles.Select(role=>new Claim(ClaimTypes.Role, role)));

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),               //After 7 days our token will expire and we'll need to login again to get a new token
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}