using BRoles.Data;
using BRoles.UtilityService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddLog4Net();
//log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oath2", new OpenApiSecurityScheme
    {
        
        Description = "Standard Authorization header using the Bearer scheme(\"bearer{token}\")",
        In=ParameterLocation.Header,
        Name="Authorozation",
        Type=SecuritySchemeType.ApiKey

    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();

});
builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer=false,
            ValidateAudience=false
        };
    }
    );


builder.Services.AddDbContext<FullStackDBContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("FullStackConnectionString")
));




builder.Services.AddDbContext<OperationsDBContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("OperationConnectionString")
));


//dependency injection
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<Icheck, check>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();//default view 
app.Run();//tous ce qui va venir apres celle ligne ne va pas s'executer 
