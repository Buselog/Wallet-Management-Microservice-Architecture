using Customer.Application.DependencyResolvers;
using Customer.Persistence.DependencyResolvers;
using Customer.InnerInfrastructure.DependencyResolvers;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContextService(builder.Configuration);
builder.Services.AddAuthenticationService(builder.Configuration);
builder.Services.AddMapperService();
builder.Services.AddPersistenceServices();
builder.Services.AddManagerServices();


var app = builder.Build();

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
