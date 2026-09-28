using Document.Api.Middlewares;
using Document.Application.Services;
using Document.InnerInfrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Key"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),

        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],

        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Document API", Version = "v1" });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Örnek: 'Bearer {token}'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    });


var retryCount = builder.Configuration.GetValue<int>("OpenRouterSettings:MaxRetryCount", 2);

IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError() 
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests) 
        .WaitAndRetryAsync(
            retryCount: retryCount,
            sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))
        );
}

builder.Services.AddHttpClient<IDocumentOcrClient, OpenRouterOcrClient>(client =>
{
    var timeout = builder.Configuration.GetValue<int>("OpenRouterSettings:TimeoutSeconds", 40);
    client.Timeout = TimeSpan.FromSeconds(timeout);
}).AddPolicyHandler(GetRetryPolicy());

builder.Services.AddScoped<IDocumentNormalizer, DocumentNormalizer>();
builder.Services.AddScoped<IDocumentParserService, DocumentParserService>();


builder.Services.AddHttpClient<IWalletClient, WalletClient>(client =>
{
    var walletUrl = builder.Configuration["WalletSettings:BaseUrl"] ?? "http://localhost:5176/";
    client.BaseAddress = new Uri(walletUrl);

    var timeout = builder.Configuration.GetValue<int>("WalletSettings:TimeoutSeconds", 25);
    client.Timeout = TimeSpan.FromSeconds(timeout);
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<IWalletClient, WalletClient>((sp, client) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var walletUrl = configuration["ExternalServices:WalletApiUrl"] ?? "https://localhost:7012/";
    client.BaseAddress = new Uri(walletUrl);
    client.Timeout = TimeSpan.FromSeconds(15);
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

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

app.Run();


