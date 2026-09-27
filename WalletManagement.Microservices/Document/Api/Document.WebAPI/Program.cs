using Document.Api.Middlewares;
using Document.Application.Services;
using Document.InnerInfrastructure.Services;
using Polly;
using Polly.Extensions.Http;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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

app.UseAuthorization();

app.MapControllers();

app.Run();


