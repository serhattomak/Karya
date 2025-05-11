using Karya.Domain.Entities;
using Karya.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using File = Karya.Domain.Entities.File;

namespace Karya.Persistence.Context;

public class AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : IdentityDbContext<AppUser, AppRole, Guid>(options)
{
	public DbSet<Product> Products => Set<Product>();
	public DbSet<File> Files => Set<File>();

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.ApplyConfigurationsFromAssembly(typeof(AppIdentityDbContext).Assembly);
	}
}