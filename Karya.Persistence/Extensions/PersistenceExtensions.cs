using Karya.Domain.Interfaces;
using Karya.Persistence.Context;
using Karya.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Karya.Persistence.Extensions;

public static class PersistenceExtensions
{
	public static IServiceCollection AddPersistenceExtensions(this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddDbContext<AppDbContext>(options =>
			options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

		services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

		var assembly = typeof(PersistenceExtensions).Assembly;
		var repositoryTypes = assembly.GetTypes()
			.Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Repository"))
			.ToList();

		foreach (var implementationType in repositoryTypes)
		{
			var interfaceType = implementationType.GetInterfaces()
				.FirstOrDefault(i => i.Name == $"I{implementationType.Name}");
			if (interfaceType != null)
			{
				services.AddScoped(interfaceType, implementationType);
			}
		}

		return services;
	}
}