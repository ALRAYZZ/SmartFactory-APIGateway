var builder = WebApplication.CreateBuilder(args);

// Adding YARP reverse proxy
builder.Services.AddReverseProxy()
	.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy")); // We add the appsettings file to the configuration that we called "ReverseProxy" in the .json

var app = builder.Build();

// Map YARP routes
app.MapReverseProxy();


app.Run();
