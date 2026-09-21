using Document.Api.Middlewares;
using Document.Application.Services;
using Document.Application.Settings;
using Document.InnerInfrastructure.Services;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var datalabConfig = builder.Configuration.GetSection("DatalabSettings").Get<DatalabSettings>();

var retryCount = builder.Configuration.GetValue<int>("GeminiSettings:MaxRetryCount", 3);

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

builder.Services.AddHttpClient<IDocumentOcrClient, GeminiOcrClient>(client =>
{
    var timeout = builder.Configuration.GetValue<int>("GeminiSettings:TimeoutSeconds", 40);
    client.Timeout = TimeSpan.FromSeconds(timeout);
}).AddPolicyHandler(GetRetryPolicy());

builder.Services.AddScoped<IDocumentParserService, DocumentParserService>();

builder.Services.AddHttpClient<IWalletClient, WalletClient>(client =>
{
    var walletUrl = builder.Configuration["WalletSettings:BaseUrl"] ?? "http://localhost:5176/";
    client.BaseAddress = new Uri(walletUrl);

    var timeout = builder.Configuration.GetValue<int>("WalletSettings:TimeoutSeconds", 25);
    client.Timeout = TimeSpan.FromSeconds(timeout);
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


