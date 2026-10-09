using System.Net;
using Konscious.Security.Cryptography;
using APCServer.Clients;
using APCServer.Data;
using APCServer.Services;
using APCServer.Workers;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Authentication.Cookies;

try
{
    Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File($"Log/{DateTime.Now}.txt")
                .CreateLogger();

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.Services.AddDbContext<ApcDbContext>(x 
                                            => x.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();
    builder.Services.AddScoped<ILoginService, LoginService>();
    builder.Services.AddTransient<IHashService, Argon2HashService>();
    builder.Services.AddSerilog();
    builder.Services.AddHttpClient<CodewarsClient>(x => x.BaseAddress = new Uri("https://www.codewars.com/api/v1/"));
    builder.Services.AddHostedService<CodewarsPollingWorker>();
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(x => {   
        x.Cookie.Name = "APCAuth";
        x.Cookie.HttpOnly = true;
        x.Cookie.SameSite = SameSiteMode.Strict;
        x.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

        x.Events.OnRedirectToLogin = y =>
        {
            y.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };

        x.Events.OnRedirectToAccessDenied = y =>
        {
            y.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseAuthentication();
    app.UseAuthorization();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApcDbContext>();
        db.Database.Migrate();
        db.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
    }


    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Failed to start");
}
finally
{
    
}


