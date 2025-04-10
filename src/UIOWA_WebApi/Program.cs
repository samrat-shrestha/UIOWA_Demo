using Application.Interfaces;
using Application.Receipts;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.FileStorage;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Application.Common.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ----- Services -----
var conn = builder.Configuration.GetConnectionString("Default") ?? "Data Source=app.db";
Console.WriteLine($"CS: {conn}");

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(conn));

builder.Services.AddScoped<IReceiptRepository, ReceiptRepository>();
builder.Services.AddScoped<IFileStore, LocalFileStore>();

builder.Services.AddMediatR(typeof(SubmitReceiptCommand).Assembly);

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<SubmitReceiptValidator>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ----- Pipeline -----
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngularApp");

// simple api key test
app.Use(async (context, next) =>
{
    // skip for OPTIONS req
    if (context.Request.Method == "OPTIONS")
    {
        await next();
        return;
    }

    // skip for Swagger
    if (context.Request.Path.StartsWithSegments("/swagger"))
    {
        await next();
        return;
    }

    if (!context.Request.Headers.TryGetValue("X-API-Key", out var apiKeyValues) || 
        apiKeyValues.FirstOrDefault() != "demo-key")
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Unauthorized. Missing or invalid API key.");
        return;
    }

    await next();
});

app.UseHttpsRedirection();
app.MapControllers();

// Ensure DB & schema
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();