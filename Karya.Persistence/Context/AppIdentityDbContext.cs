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
	public DbSet<Contact> Contacts => Set<Contact>();
	public DbSet<Page> Pages => Set<Page>();
	public DbSet<HomePage> HomePages => Set<HomePage>();
	public DbSet<AboutPage> AboutPages => Set<AboutPage>();
	public DbSet<ContactPage> ContactPages => Set<ContactPage>();

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.ApplyConfigurationsFromAssembly(typeof(AppIdentityDbContext).Assembly);
	}
}