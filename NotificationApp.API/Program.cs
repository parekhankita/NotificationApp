using MediatR;
using NotificationApp.Application;
using NotificationApp.Domain.Contracts;
using NotificationApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

// Register MediatR
builder.Services.AddMediatR(typeof(ProcessNotificationHandler).Assembly);

// Register service dependencies
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ILLMService, LLMService>();
builder.Services.AddScoped<IDiscordService, DiscordService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers(); 

app.Run();

public partial class Program { }