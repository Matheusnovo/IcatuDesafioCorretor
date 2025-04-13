using CorretorAPI.Application.Services;
using CorretorAPI.Domain.Repository.Interfaces;
using CorretorAPI.Domain.Services.Interfaces;
using CorretorAPI.Infra.Data;
using CorretorAPI.Infra.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Casa do Corretor", Version = "v1" });
});

builder.Services.AddHttpClient();

builder.Services.AddSingleton<IContratacaoRepository, ContratacaoRepository>();
builder.Services.AddScoped<IAutorizadorService, AutorizadorService>();
builder.Services.AddScoped<IContratacaoService, ContratacaoService>();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();
