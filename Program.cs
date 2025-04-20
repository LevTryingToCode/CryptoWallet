using CryptoWallet.Entities;
using CryptoWallet.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//Controllers and Services
builder.Services.AddControllers();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<ITradeService,TradeService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();

//Background Services
builder.Services.AddHostedService<CurrencyUpdateBackgroundService>();
builder.Services.AddSingleton<CurrencyLoggerBackgroundService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<CurrencyLoggerBackgroundService>());

//Database
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Sqlexpress")));

builder.Services.AddEndpointsApiExplorer();

//Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo() { Title = "CryptoWallet API", Version = "v1" });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CryptoWallet API v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
