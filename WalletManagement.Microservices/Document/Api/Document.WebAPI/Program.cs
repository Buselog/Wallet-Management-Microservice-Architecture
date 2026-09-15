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

builder.Services.AddHttpClient<IDocumentOcrClient, DatalabOcrClient>(client =>
{
    client.BaseAddress = new Uri(datalabConfig?.BaseUrl ?? "https://api.datalab.to/");
    client.Timeout = TimeSpan.FromSeconds(datalabConfig?.TimeoutSeconds ?? 30);

    if (!string.IsNullOrEmpty(datalabConfig?.ApiKey))
    {
        client.DefaultRequestHeaders.Add("Api-Key", datalabConfig.ApiKey);
    }

});
    builder.Services.AddScoped<IDocumentParserService, DocumentParserService>();

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


