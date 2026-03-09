using Wallet.Application.DependencyResolvers;
using Wallet.Persistence.DependencyResolvers;
using Wallet.InnerInfrastructure.DependencyResolvers;
using Wallet.WebAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContextService(builder.Configuration);
builder.Services.AddMapperService();
builder.Services.AddRepositoryServices();
builder.Services.AddManagerServices();
builder.Services.AddExceptionHandler<ExceptionMiddleware>();


var app = builder.Build();

app.UseExceptionHandler();

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
