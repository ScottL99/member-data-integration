using System.Net;
using MemberDataIntegration.Api.Clients;
using MemberDataIntegration.Api.Data;
using MemberDataIntegration.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// IMemberPress
builder.Services
    .AddHttpClient<IMemberSourceClient, MemberPressClient>()
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var proxyUrl = builder.Configuration["MemberPress:ProxyUrl"];

        if (string.IsNullOrWhiteSpace(proxyUrl))
        {
            return new HttpClientHandler();
        }

        return new HttpClientHandler
        {
            Proxy = new WebProxy(proxyUrl),
            UseProxy = true,
        };
    });

// Services
builder.Services.AddScoped<MemberService>();

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Build ----------------------------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
