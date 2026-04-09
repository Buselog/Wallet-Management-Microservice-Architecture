using Customer.Application.DependencyResolvers;
using Customer.Application.Managers;
using Customer.InnerInfrastructure.DependencyResolvers;
using Customer.InnerInfrastructure.Services;
using Customer.Persistence.DependencyResolvers;
using Customer.WebAPI.Filters;
using Customer.WebAPI.Middlewares;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseSerilog();
builder.Services.AddLoggerService(builder.Configuration);
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContextService(builder.Configuration);
builder.Services.AddAuthenticationService(builder.Configuration);
builder.Services.AddMapperService();
builder.Services.AddPersistenceServices();
builder.Services.AddManagerServices();
builder.Services.AddValidatorServices();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddHttpClient<IWalletServiceClient, WalletServiceClient>(options =>
{
    var baseUrl = builder.Configuration.GetSection("WalletApiSwaggerAddress:BaseUrl").Value;
    options.BaseAddress = new Uri(baseUrl ?? "https://localhost:7012/");
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",new OpenApiInfo
    {
        Title = "Wallet Management System - Customer API",
        Description = "4ARC Software - Microservice Architecture Customer API"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
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

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
