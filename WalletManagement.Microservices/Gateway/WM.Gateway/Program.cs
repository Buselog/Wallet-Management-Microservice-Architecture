using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using System.Text;
using Serilog;
using WM.Gateway.Handlers;
using WM.Gateway.Services.Abstracts;
using WM.Gateway.Services.Concretes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient("CustomerAPI", c => { 
    c.BaseAddress = new Uri("https://localhost:7145/"); 
});

builder.Services.AddHttpClient("WalletAPI", c => { 
    c.BaseAddress = new Uri("https://localhost:7012/"); 
});

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options => 
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddHttpClient("MultiLanguageAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7207/"); 
});

builder.Services.AddTransient<ResilienceHandler>();
builder.Services.AddTransient<LocalizationHandler>();
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddSwaggerForOcelot(builder.Configuration);
builder.Services.AddOcelot()
    .AddDelegatingHandler<LocalizationHandler>()
    .AddDelegatingHandler<ResilienceHandler>()
    .AddPolly();

builder.Services.AddScoped<IDashboardService, DashboardService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()   
               .AllowAnyMethod()   
               .AllowAnyHeader();  
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
}

app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/v1/swagger.json";
});


app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); 
});


await app.UseOcelot();

app.Run();