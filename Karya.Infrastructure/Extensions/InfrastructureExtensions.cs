using Karya.Infrastructure.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace Karya.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
	public static IServiceCollection AddInfrastructureExtensions(this IServiceCollection services)
	{
		var assembly = typeof(InfrastructureExtensions).Assembly;
		var serviceTypes = assembly.GetTypes()
			.Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service"))
			.ToList();

		foreach (var implementationType in serviceTypes)
		{
			var interfaceType = implementationType.GetInterfaces()
				.FirstOrDefault(i => i.Name == $"I{implementationType.Name}");
			if (interfaceType != null)
			{
				services.AddScoped(interfaceType, implementationType);
			}
		}

		services.AddAutoMapper(typeof(GeneralMapping));
		return services;
	}
}