using Microsoft.AspNetCore.Authentication.BearerToken;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MessageReceivedContext = Microsoft.AspNetCore.Authentication.JwtBearer.MessageReceivedContext;
using Model.Entities.PruebaDoubleV.Sql;

namespace WebApi.Security
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration configuration;
        private readonly JwtSecurityTokenHandler tokenHandler;
        public JwtService(IConfiguration configuration)
        {
            this.configuration = configuration;
            tokenHandler = new JwtSecurityTokenHandler();
        }

        public Task Authenticate()
        {
            return Task.CompletedTask;
        }


        public JwtResult CreateJwtToken(List<Claim>? claims = null)
        {
            claims = claims ?? new List<Claim>();
            var jti = Guid.NewGuid().ToString();

            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, jti));

            var minutes = configuration.GetValue<int>("SecurityOptions:TokenMinutesAlive");
            DateTime expiration = DateTime.UtcNow;
            expiration = expiration.AddMilliseconds(minutes);

            var secretKey = configuration.GetValue<string>("SecurityOptions:TokenSecretKey");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "Issuer",
                Audience = "Audience",
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)), SecurityAlgorithms.HmacSha512),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new JwtResult
            {
                Token = tokenHandler.WriteToken(token),
                Expiration = expiration,
                Jti = jti
            };

        }
        public JwtSecurityToken ValidateJwtToken(string token)
        {
            var secretKey = configuration.GetValue<string>("SecurityOptions:TokenSecretKey");

            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };
            tokenHandler.ValidateToken(token, validationParameters, out var securityToken);
            if (!(securityToken is JwtSecurityToken jwtToken && jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase)))
            {
                return null!;
            }
            return jwtToken;
        }
        public static async Task OnTokenValidated(TokenValidatedContext context)
        {
            try
            {
                if (!(context.SecurityToken is JwtSecurityToken jwtToken && jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase)))
                {
                    context.Fail(Common.Type.ResultType.SessionFail.ToString());
                    await Task.CompletedTask;
                    return;
                }

                var dbContext = context.HttpContext.RequestServices.GetService(typeof(DatabaseContext)) as DatabaseContext;
                if (dbContext == null)
                {
                    context.Fail(Common.Type.ResultType.ContextFail.ToString());
                    await Task.CompletedTask;
                    return;
                }

            }
            catch
            {
                context.Fail(Common.Type.ResultType.InternalError.ToString());
            }
            return;
        }
        public static Task OnMessageReceived(MessageReceivedContext context)
        {
            return Task.CompletedTask;
        }
        public static Task OnChallenge(JwtBearerChallengeContext arg)
        {
            if (arg.AuthenticateFailure is SecurityTokenExpiredException)
            {
                arg.Error = Common.Type.ResultType.SessionFail.ToString();
                return Task.CompletedTask;
            }
            arg.Error = arg.AuthenticateFailure != null ? arg.AuthenticateFailure.Message : Common.Type.ResultType.SessionFail.ToString();
            return Task.CompletedTask;
        }
        public static Task OnAuthenticationFailed(AuthenticationFailedContext arg)
        {
            return Task.CompletedTask;
        }
        public static Task OnForbidden(ForbiddenContext arg)
        {
            return Task.CompletedTask;
        }
    }
}
