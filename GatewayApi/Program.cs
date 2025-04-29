using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers
builder.Services.AddControllers();

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters()
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidAudience = builder.Configuration["Jwt:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
	};
});

// Adding YARP reverse proxy
builder.Services.AddReverseProxy()
	.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy")); // We add the appsettings file to the configuration that we called "ReverseProxy" in the .json

// Adding Rate Limiting
builder.Services.AddRateLimiter(options =>
{
	options.AddFixedWindowLimiter("GlobalLimit", policyOptions =>
	{
		policyOptions.PermitLimit = 100; // N. of requests allowed in the time window
		policyOptions.Window = TimeSpan.FromMinutes(1); // Time window
		policyOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // Order in which requests are processed
		policyOptions.QueueLimit = 10; // N. of requests that can be queued
	});

	// Response when hitting rate limit
	options.OnRejected = async (context, cancellationToken) =>
	{
		context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests; // Too many requests
		await context.HttpContext.Response.WriteAsync("Rate limit exceeded. Please try again later.", cancellationToken); // Response message
	};
});



var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRateLimiter(); // Enable rate limiting
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireRateLimiting("GlobalLimit");


// Map YARP routes
app.MapReverseProxy(proxyPipeline =>
{
	proxyPipeline.Use(async (context, next) =>
	{
		// Skip authentication for the login endpoint
		if (context.Request.Path.StartsWithSegments("/login"))
		{
			await next(); // Skip authentication for this request
			return;
		}

		// Require authentication for other routes
		if (!context.User.Identity.IsAuthenticated)
		{
			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			return; // Return 401 Unauthorized if not authenticated
		}

		await next(); // Continue processing the request
	});
}).RequireRateLimiting("GlobalLimit");


app.Run();
