using SocialListening.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddMyServices();
builder.Services.AddHttpLogging(o => { });

var redditSettings = builder.Configuration.GetSection("RedditConfig");
var redditConfig = redditSettings.Get<RedditConfig>();
if (redditConfig != null) builder.Services.AddSingleton(redditConfig);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
