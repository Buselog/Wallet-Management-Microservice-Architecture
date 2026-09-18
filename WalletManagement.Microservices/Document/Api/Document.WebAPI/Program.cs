using Document.Api.Middlewares;
using Document.Application.Services;
using Document.Application.Settings;
using Document.InnerInfrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var datalabConfig = builder.Configuration.GetSection("DatalabSettings").Get<DatalabSettings>();


builder.Services.AddHttpClient<IDocumentOcrClient, GeminiOcrClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(40);
});

builder.Services.AddScoped<IDocumentParserService, DocumentParserService>();

builder.Services.AddHttpClient<IWalletClient, WalletClient>(client =>
{
    var walletUrl = builder.Configuration["WalletSettings:BaseUrl"] ?? "http://localhost:5176/";
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


