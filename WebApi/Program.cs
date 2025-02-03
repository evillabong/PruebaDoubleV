using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Model.Entities.PruebaDoubleV.Sql;
using System.Text;
using WebApi.Security;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;
IWebHostEnvironment _environment = builder.Environment;
// Add services to the container.
var connectionString = configuration.GetConnectionString("SqlServerConnectionString");

builder.Logging
     .AddDebug();

builder.Services.AddCors(options =>
{
    List<PoliceSettings> policies = new List<PoliceSettings>();
    configuration.Bind("SecurityOptions:Cors", policies); ;
    foreach (var police in policies)
    {
        options.AddPolicy(police.Name,
            policy =>
            {
                if (police.Origin != null)
                {
                    policy.WithOrigins(police.Origin.Distinct().ToArray());
                }
                else
                {
                    policy.AllowAnyOrigin();
                }
                policy
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            });
    }


});
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(connectionString);
    if (!_environment.IsProduction())
    {
        options.EnableSensitiveDataLogging();
        builder.Logging
        .AddConsole();
    }
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(options =>
                        {
                            var secretKey = configuration.GetValue<string>("SecurityOptions:TokenSecretKey")!;
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = false,
                                ValidateAudience = false,
                                ValidateIssuerSigningKey = true,
                                ValidateLifetime = true,
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                                ClockSkew = TimeSpan.Zero,
                                RequireExpirationTime = true,
                            };

                            options.Events = new JwtBearerEvents
                            {
                                OnMessageReceived = JwtService.OnMessageReceived,
                                OnTokenValidated = JwtService.OnTokenValidated,
                                OnAuthenticationFailed = JwtService.OnAuthenticationFailed,
                                OnChallenge = JwtService.OnChallenge,
                                OnForbidden = JwtService.OnForbidden,
                            };

                        });

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IJwtService, JwtService>((provider) => new JwtService(provider.GetService<IConfiguration>()!));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
