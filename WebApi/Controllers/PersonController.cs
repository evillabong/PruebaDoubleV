using Azure;
using Common.Base;
using Common.Result;
using Common.Type;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Model.Entities.PruebaDoubleV.Sql;
using Model.Type;
using System.Data;
using WebApi.Security;

namespace WebApi.Controllers
{
    [EnableCors(PolicyName = "DoubleVPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        public DatabaseContext _dbContext;
        public IConfiguration _configuration;
        public IJwtService _jwtService;
        public ILogger<PersonController> _logger;
        public PersonController(DatabaseContext dbContext, IConfiguration configuration, IJwtService jwtService, ILogger<PersonController> logger)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _jwtService = jwtService;
            _logger = logger;
        }

        [Authorize]
        //[AllowAnonymous]
        [HttpGet(nameof(Common.Controllers.Person.GetPersons))]
        public async Task<GetPersonsResult> GetResultAsync()
        {
            var ret = new GetPersonsResult();
            try
            {
                var resultCodeParam = new SqlParameter("@ResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var messageParam = new SqlParameter("@Message", SqlDbType.VarChar, 150) { Direction = ParameterDirection.Output };

                var persons = await _dbContext
                    .PersonModels
                    .FromSqlRaw ("EXECUTE dbo.PersonManager @Operation = {0}, @ResultCode = {1} OUT, @Message = {2} OUT",
                        "C", resultCodeParam, messageParam)
                    .ToListAsync();
                if(persons.Count > 0)
                {
                    ret.Persons = persons.Select(p => new PersonBase
                    {
                        Id = p.Id,
                        Identification = p.Identification,
                        Name = p.Name,
                        Username = p.Username,
                        Email = p.Email,
                        DateTimeOffSet = p.CreatedAt,
                    }).ToList();
                }
                ret.ResultCode = (ResultType)(int)resultCodeParam.Value;
                ret.Message = (string)messageParam.Value;
                return ret;
            }
            catch (Exception ex)
            {
                ret.ResultCode = Common.Type.ResultType.InternalError;
                ret.Message = "Fatal error.";
                _logger.LogError(ex, Common.Type.ResultType.InternalError.ToString());
                return ret;
            }


        }
    }
}
