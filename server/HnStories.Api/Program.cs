using Polly;
using Polly.Extensions.Http;
using HnStories.Api.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var clientOrigin = builder.Configuration["ClientOrigin"] ?? "http://localhost:4200";
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .WithOrigins(clientOrigin)
    .AllowAnyHeader()
    .AllowAnyMethod()));

builder.Services.AddControllers();

builder.Services.AddHttpClient<IHnClient, HnClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Hn:BaseUrl"]!);
})
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))
    )
);

builder.Services.AddScoped<IStoryService, StoryService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HN Newest API", Version = "v1" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.MapControllers();

app.Run();

public partial class Program { }
