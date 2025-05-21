var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<ServiceBusConsumer>();
builder.Services.AddHttpClient<MLInferenceService>();
builder.Services.AddSingleton<SneakerSearchService>();

builder.Services.AddSingleton<ServiceBusClient>(sp =>
    new ServiceBusClient(builder.Configuration["Azure:ServiceBusConnection"]));

builder.Services.AddSingleton<ServiceBusSender>(sp =>
    sp.GetRequiredService<ServiceBusClient>()
        .CreateSender(builder.Configuration["Azure:ResponseQueue"]));

builder.Services.AddControllers();
var app = builder.Build();

app.MapControllers();
app.Run();
