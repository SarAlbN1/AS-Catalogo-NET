using KafkaConsumer;
using KafkaConsumer.Services;

var builder = Host.CreateApplicationBuilder(args);

// Registrar servicios
builder.Services.AddSingleton<EmailService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
