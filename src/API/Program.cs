using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;

public class Program
{
    public static IConfiguration StaticConfig { get; private set; }
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var allowedOrigins = builder.Configuration.GetValue<string>("Origins").Split(',');

        StaticConfig = builder.Configuration;

        builder.Services.AddCors();

        builder.Services.AddAuthentication(config =>
        {
            config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
           .AddJwtBearer(bearer =>
           {
               bearer.Authority = builder.Configuration.GetSection("IdentityServerSettings").GetSection("Authority").Value.ToString();
               bearer.Audience = builder.Configuration.GetSection("IdentityServerSettings").GetSection("Audience").Value.ToString();
               bearer.RequireHttpsMetadata = true;

           });



        builder.Services.AddControllers();

        builder.Services.AddHttpLogging(o => { });

        var app = builder.Build();

        app.UseRouting();

        app.UseExceptionHandler("/error");

        app.UseCors(c => c.AllowCredentials()
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                             //    .AllowAnyOrigin()
                             //  .SetIsOriginAllowed(s => allowedOrigins.Any(a => a.ToLower() == s.ToLower()))
                             .SetIsOriginAllowed(s => true)
                               );

        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.UseHttpLogging();

        app.Run();
    }
}
