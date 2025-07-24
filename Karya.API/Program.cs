using Karya.API.Extensions;
using Karya.Application.Extensions;
using Karya.Infrastructure.Extensions;
using Karya.Persistence.Context;
using Karya.Persistence.Extensions;
using Karya.Persistence.Seed;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowReact",
		policy => policy.WithOrigins("http://localhost:3000")
			.AllowAnyHeader()
			.AllowAnyMethod());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
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

app.UseCors("AllowReact");

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();