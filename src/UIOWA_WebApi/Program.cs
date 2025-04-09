using Application.Interfaces;
using Application.Receipts;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.FileStorage;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ----- Services -----
var conn = builder.Configuration.GetConnectionString("Default") ?? "Data Source=app.db";
Console.WriteLine($"CS: {conn}");

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(conn));

builder.Services.AddScoped<IReceiptRepository, ReceiptRepository>();
builder.Services.AddScoped<IFileStore, LocalFileStore>();

builder.Services.AddMediatR(typeof(SubmitReceiptCommand).Assembly);

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

app.UseHttpsRedirection();
app.MapControllers();

// Ensure DB & schema
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();