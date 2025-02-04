using Common.Base;
using Common.Param;
using Common.Result;
using Common.Type;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Entities.PruebaDoubleV.Sql;
using System.Data;
using WebApi.Security;

namespace WebApi.Controllers
{
    [EnableCors(PolicyName = "DoubleVPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public DatabaseContext _dbContext;
        public IConfiguration _configuration;
        public IJwtService _jwtService;
        public ILogger<UserController> _logger;
        public UserController(DatabaseContext dbContext, IConfiguration configuration, IJwtService jwtService, ILogger<UserController> logger)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _jwtService = jwtService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost(nameof(Common.Controllers.User.SetRegistrationUser))]
        public async Task<SetRegistrationUserResult> SetRegistrationUserAsync([FromBody] SetRegistrationUserParam param)
        {
            var ret = new SetRegistrationUserResult();
            var query = await _dbContext.User.FirstOrDefaultAsync(p => p.Username == param.Username);
            if (query == null)
            {
                using (var trasnsaction = await _dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        Person person = new Person
                        {
                            Identification = param.Identification,
                            IdentificationType = param.IdentificationType.ToString(),
                            Firstname = param.Firstname,
                            Lastname = param.Lastname,
                            Email = param.Email
                        };
                        await _dbContext.Person.AddAsync(person);
                        await _dbContext.SaveChangesAsync();

                        var user = new User
                        {
                            Username = param.Username,
                            Password = await SecurityModel.GetHash512Async(param.Password),
                            PersonId = person.Id
                        };
                        await _dbContext.SaveChangesAsync();
                        await _dbContext.Database.CommitTransactionAsync();

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, nameof(UserController));
                        await _dbContext.Database.RollbackTransactionAsync();
                        ret.ResultCode = ResultType.InternalError;
                        ret.Message = "Transaction fail, retry again";
                    }
                }
            }
            return ret;
        }
    }
}
