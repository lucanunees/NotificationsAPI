using Microsoft.OpenApi;
using NotificationsAPI.HostedServices;
using NotificationsAPI.Infrastructure.DependencyInjection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Registra todos os serviços (Infrastructure + Application) no container de DI
builder.Services.AddInfrastructure(builder.Configuration);

// Registra o serviço de fundo que consome mensagens do RabbitMQ
builder.Services.AddHostedService<RabbitMQConsumerHostedService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("V1", new OpenApiInfo
    {
        Title = "FIAP Cloud Games (FCG)",
        Version = "V1",
        Description = "API de Notificações do FIAP Cloud Games (FCG) - Envio de e-mails de boas-vindas e confirmações de compra.",
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/V1/swagger.json", "FIAP Cloud Games (FCG) V1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();