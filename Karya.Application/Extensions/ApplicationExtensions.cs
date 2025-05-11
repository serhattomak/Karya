using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Karya.Application.Extensions;

public static class ApplicationExtensions
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services)
	{
		services.AddAutoMapper(Assembly.GetExecutingAssembly());
		var assembly = typeof(ApplicationExtensions).Assembly;
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
		return services;
	}
}