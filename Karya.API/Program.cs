using Karya.API.Extensions;
using Karya.Application.Extensions;
using Karya.Infrastructure.Extensions;
using Karya.Persistence.Context;
using Karya.Persistence.Extensions;
using Karya.Persistence.Seed;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
	builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

builder.Services.AddControllers();
builder.Services.AddJwtAuthentication(builder.Configuration);

// OpenAPI with JWT Security
builder.Services.AddOpenApi(options =>
{
	options.AddDocumentTransformer((document, context, cancellationToken) =>
	{
		document.Info = new OpenApiInfo
		{
			Title = "Karya API",
			Version = "v1",
			Description = "Karya Project API with JWT Authentication"
		};

		// JWT Security Scheme
		document.Components ??= new OpenApiComponents();
		document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
		{
			["Bearer"] = new OpenApiSecurityScheme
			{
				Type = SecuritySchemeType.Http,
				Scheme = "bearer",
				BearerFormat = "JWT",
				Description = "Enter your JWT token in the format: Bearer {token}"
			}
		};

		// Global security requirement
		document.SecurityRequirements = new List<OpenApiSecurityRequirement>
		{
			new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						}
					},
					Array.Empty<string>()
				}
			}
		};

		return Task.CompletedTask;
	});
});

builder.Services.AddAuthorization();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureExtensions();
builder.Services.AddPersistenceExtensions(builder.Configuration);

var allowedOrigins = builder.Configuration["Cors__AllowedOrigins"];
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowWeb", policy =>
		policy
			.SetIsOriginAllowed(origin =>
			{
				try
				{
					if (!string.IsNullOrWhiteSpace(allowedOrigins))
					{
						var host = new Uri(origin).Host;
						return allowedOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
							.Any(p => host.Equals(new Uri(p).Host, StringComparison.OrdinalIgnoreCase)
									  || host.EndsWith(new Uri(p).Host, StringComparison.OrdinalIgnoreCase));
					}
					var h = new Uri(origin).Host;
					return h.EndsWith(".vercel.app", StringComparison.OrdinalIgnoreCase)
						   || h.EndsWith(".railway.app", StringComparison.OrdinalIgnoreCase)
						   || h.Equals("localhost", StringComparison.OrdinalIgnoreCase)
						   || h.EndsWith(".localhost", StringComparison.OrdinalIgnoreCase);
				}
				catch { return false; }
			})
			.AllowAnyHeader()
			.AllowAnyMethod()
			.DisallowCredentials()
	);
});

builder.Services.Configure<FormOptions>(options =>
{
	options.ValueLengthLimit = int.MaxValue;
	options.MultipartBodyLengthLimit = int.MaxValue;
	options.MultipartHeadersLengthLimit = int.MaxValue;
});

builder.Services.Configure<IISServerOptions>(options =>
{
	options.MaxRequestBodySize = 100 * 1024 * 1024;
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
	options.Limits.MaxRequestBodySize = 100 * 1024 * 1024;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	await context.Database.MigrateAsync();
	await UserSeed.SeedAsync(context);
}

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference(options =>
	{
		options
			.WithTitle("KaryaAPI")
			.WithTheme(ScalarTheme.Purple)
			.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
	});
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
	ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
});

app.UseCors("AllowWeb");

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapControllers();

app.Run();