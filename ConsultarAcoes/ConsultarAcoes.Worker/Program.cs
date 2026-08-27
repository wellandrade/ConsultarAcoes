using Azure.Messaging.ServiceBus;
using ConsultarAcoes.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var connectionString = builder.Configuration["AzureServiceBus:ConnectionString"];

builder.Services.AddSingleton(new ServiceBusClient(connectionString));

var host = builder.Build();
host.Run();
