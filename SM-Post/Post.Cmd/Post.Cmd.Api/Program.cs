using CQRS.Core.Domain;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Cmd.Api;
using Post.Cmd.Infrastructure.Config;
using Post.Cmd.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Call ConfigureServices to add services to the container
ConfigureServices(builder.Services);

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

void ConfigureServices(IServiceCollection services)
{
    // Add services to the container. This method gets called by the runtime. Use this method to add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Get the MongoDB Configuration from the appsettings.json file (.development, .production, .staging)
    services.Configure<MongoDbConfig>(builder.Configuration.GetSection("MongoDbConfig"));

    // Register your services here
    services.AddSingleton<IMongoClient>(sp =>
    {
        var config = sp.GetRequiredService<IOptions<MongoDbConfig>>();
        return new MongoClient(config.Value.ConnectionString);
    });

    services.AddSingleton(sp =>
    {
        var mongoClient = sp.GetRequiredService<IMongoClient>();
        var config = sp.GetRequiredService<IOptions<MongoDbConfig>>();
        var database = mongoClient.GetDatabase(config.Value.Database);
        return database.GetCollection<EventModel>("EventStore");
    });

    services.AddScoped<IEventStoreRepository, EventStoreRepository>();

    // Other service registrations...
}
