using AutoMapper;
using Mango.Services.CouponAPI;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography.Xml;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(option => {
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

IMapper mapper = MappingConfig.registerMaps().CreateMapper(); 
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());




builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


//using this configuration we can apply restriction or Lock to API and can
//access by only With Providing token to Authorize and then can Access it
// => Bearer <token> 
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the bearer Authorization string as following => Bearer 'Genereted-jwt-token'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    { {
        new OpenApiSecurityScheme
        { 
            Reference=new OpenApiReference
            {   
                Type =ReferenceType.SecurityScheme,
                Id=JwtBearerDefaults.AuthenticationScheme 
            }
        },new string []{ }  
  
        }
    });

});

/* --This code is Addd in to seperate class for avoid complex clustering and to clean code in Extension folder

// this configuration for Authorize the user for access certain functionality after
// this configuration we can add [Authoize] attriute in API controller to restrict for
// authorized user can access that API and display Unauthorized message else.

var secret = builder.Configuration.GetValue<string>("ApiSetting:Secret");
var Issuer = builder.Configuration.GetValue<string>("ApiSetting:Issuer");
var Audience = builder.Configuration.GetValue<string>("ApiSetting:Audience");

var key = Encoding.ASCII.GetBytes(secret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey =true,
        IssuerSigningKey= new SymmetricSecurityKey(key),
        ValidateIssuer=true,
        ValidIssuer=Issuer,
        ValidateAudience=true,
        ValidAudience=Audience
    };

});
*/
//call  from extensions folder
builder.AddAuthentication();

builder.Services.AddAuthorization();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

ApplyMigration();

app.Run();

void ApplyMigration() {

    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (_db.Database.GetPendingMigrations().Count()>0) { 
        
            _db.Database.Migrate();
        }

    }
}
