using Common.Param;
using Common.Result;
using Common.Type;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Model;
using Model.Entities.PruebaDoubleV.Sql;
using System.Security.Claims;
using System.Text;
using WebApi.Extensions;
using WebApi.Security;
using Common.Extensions;
using Microsoft.AspNetCore.Cors;

namespace WebApi.Controllers
{
    [EnableCors(PolicyName = "DoubleVPolicy")]
    [Route("api/[controller]")]   
    [ApiController]
    public class SecurityController : ControllerBase
    {
        public DatabaseContext _dbContext;
        public IConfiguration _configuration;
        public IJwtService _jwtService;
        public SecurityController(DatabaseContext dbContext, IConfiguration configuration, IJwtService jwtService)
        {
            this._dbContext = dbContext;
            this._configuration = configuration;
            this._jwtService = jwtService;
        }

        [AllowAnonymous]
        [HttpPost(nameof(Common.Controllers.Security.GetLogin))]
        public async Task<GetLoginResult> GetLogin([FromBody] SecureParam secureParam)
        {
            var ret = new GetLoginResult();
            var loginParam = await SecurityModel.DecryptedObjectAsync<GetLoginParam>(secureParam, _configuration.GetPrivateKey());
            var query = await _dbContext.User
                            .Include(p => p.Person)
                            .FirstOrDefaultAsync(p => p.Username == loginParam.Username);
            loginParam.Password = await SecurityModel.GetHash512Async(Encoding.UTF8.GetBytes(loginParam.Password));
            if (query != null)
            {
                if (loginParam.Password == query.Password)
                {
                    var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, query.Username),
                                new Claim(ClaimTypes.Role, RoleType.User.ToString()),
                                new Claim("CreateBy","Double V Partner"),
                            };

                    var tk = _jwtService.CreateJwtToken(claims);
                    var expiration = tk.Expiration.ToDateTimeOffSet();

                    ret.Token = tk.Token;
                    ret.TokenExpire = tk.Expiration.ToDateTimeOffSet();
                    ret.Name = query.Person!.Firstname;

                    return ret;
                }
                else
                {
                    ret.ResultCode = Common.Type.ResultType.Error;
                    ret.Message = $"Contraseña incorrecta";
                    return ret;
                }
            }
            else
            {
                ret.ResultCode = Common.Type.ResultType.Error;
                ret.Message = $"Usuario incorrecto";
                return ret;
            }
        }

        [AllowAnonymous]
        [HttpGet(nameof(Common.Controllers.Security.GetPublicKey))]
        public async Task<GetPublicKeyResult> GetPublicKey()
        {
            return await Task.Run(() =>
            {
                return new GetPublicKeyResult
                {
                    Value = _configuration.GetPublicKey()
                };
            });
        }
    }
}
