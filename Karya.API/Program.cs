using Karya.API.Extensions;
using Karya.Application.Extensions;
using Karya.Infrastructure.Extensions;
using Karya.Persistence.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddEventLog(o => o.SourceName = "KaryaAPI");

if (!builder.Environment.IsProduction())
{
	var port = Environment.GetEnvironmentVariable("PORT");
	if (!string.IsNullOrWhiteSpace(port))
	{
		builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
	}
}

builder.Services.AddControllers();
builder.Services.AddJwtAuthentication(builder.Configuration);

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

string[] fromJson =
	builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
string[] fromEnvCsv =
	(builder.Configuration["Cors__AllowedOrigins"] ?? string.Empty)
		.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
var finalAllowedOrigins =
	(fromJson.Length > 0 ? fromJson : fromEnvCsv.Length > 0 ? fromEnvCsv : new[]
	{
		"https://karyayapi.com",
		"https://www.karyayapi.com"
	})
	.Distinct(StringComparer.OrdinalIgnoreCase)
	.ToArray();

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowWeb", policy =>
		policy
			.WithOrigins(finalAllowedOrigins)
			.AllowAnyHeader()
			.AllowAnyMethod()
			.DisallowCredentials()
			.SetPreflightMaxAge(TimeSpan.FromHours(1))
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

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.Lifetime.ApplicationStarted.Register(() => Console.WriteLine("### KaryaAPI started"));
app.Lifetime.ApplicationStopping.Register(() => Console.WriteLine("### KaryaAPI stopping"));

//try
//{
//	using var scope = app.Services.CreateScope();
//	var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//	await context.Database.MigrateAsync();
//	await UserSeed.SeedAsync(context);
//}
//catch (Exception ex)
//{
//	Console.Error.WriteLine("### MIGRATION ERROR: " + ex);
//}

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

app.UseHttpsRedirection();

app.UseCors("AllowWeb");

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();