using GreekMythology.Worker;

var builder = Host.CreateApplicationBuilder(args);

// Configuration du HttpClient
builder.Services.AddHttpClient();

// Enregistrement du Worker
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
