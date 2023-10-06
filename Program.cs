using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connMux = ConnectionMultiplexer.Connect("localhost");

var rlMultiplexer = new List<RedLockMultiplexer> {
    connMux
};

var redlockFactory = RedLockFactory.Create(rlMultiplexer);


builder.Services.AddSingleton<IDistributedLockFactory>(redlockFactory);

var app = builder.Build();

app.Lifetime.ApplicationStopping.Register(() => {
    redlockFactory.Dispose();
});

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
