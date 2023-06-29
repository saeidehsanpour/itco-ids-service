using System;
using System.Reflection;
using CaterSoft.IdentityServer;
using CaterSoft.IdentityServer.DataAccess;
using CaterSoft.IdentityServer.DataAccess.Repositories;
using CaterSoft.IdentityServer.Services;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var migrationAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

builder.Services.AddScoped<DbContext, IdentityContext>();
builder.Services.AddDbContext<IdentityContext>((serviceProvider, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("sqlConnection"));
});
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserLogRepository, UserLogRepository>();

builder.Services.AddIdentityServer()
    .AddConfigurationStore(opt =>
    {
        opt.ConfigureDbContext = c => c.UseNpgsql(builder.Configuration.GetConnectionString("sqlConnection"),
            sql => sql.MigrationsAssembly(migrationAssembly));
    })
    .AddOperationalStore(opt =>
    {
        opt.ConfigureDbContext = o => o.UseNpgsql(builder.Configuration.GetConnectionString("sqlConnection"),
            sql => sql.MigrationsAssembly(migrationAssembly));
    }).AddProfileService<ProfileService>();

builder.Services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();
builder.Services.AddTransient<IProfileService, ProfileService>();

IdentityModelEventSource.ShowPII = true;
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", opt =>
    {
        opt.RequireHttpsMetadata = false;
        opt.Authority = builder.Configuration.GetSection("jwt:issuer").Value;
        opt.Audience = builder.Configuration.GetSection("jwt:issuer").Value;
    });
builder.Services.AddControllers();

//services.AddCors(options => options.AddPolicy(Policy, p => p.AllowAnyOrigin()
//    .AllowAnyMethod()
//    .AllowAnyHeader()));
var app = builder.Build();
var env = builder.Environment;

if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.ConfigureCultureHandler();
app.UseIdentityServer();
app.UseAuthentication();
app.UseRouting();
//app.UseCors(Policy);
app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", false);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
app.Run();