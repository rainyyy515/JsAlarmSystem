using isRock.LineBot;
using LineSystem.Models;
using LineSystem.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();
builder.Services.AddControllers();
builder.Services.AddSingleton<Bot>(factory =>
{
    var token = builder.Configuration.GetValue<string>("ChannelAccessToken");
    return new Bot(token);
});
builder.Services.AddSqlServer<LineAlarmContext>(builder.Configuration.GetConnectionString("JsLineAlarm"));
builder.Services.AddScoped<AlarmServer>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
