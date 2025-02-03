using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebApi.Security
{
    public interface IJwtService
    {
        JwtResult CreateJwtToken(List<Claim>? claims = null);
        JwtSecurityToken ValidateJwtToken(string token);
    }
}
