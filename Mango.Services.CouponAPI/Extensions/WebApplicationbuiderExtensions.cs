using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Mango.Services.CouponAPI.Extensions
{
    public static class WebApplicationbuiderExtensions
    {
        public static WebApplicationBuilder AddAuthentication(this WebApplicationBuilder builder)
        {
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
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = Issuer,
                    ValidateAudience = true,
                    ValidAudience = Audience
                };

            });

            return builder;

        }


    }
}