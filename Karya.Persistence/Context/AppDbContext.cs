using Karya.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using File = Karya.Domain.Entities.File;

namespace Karya.Persistence.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	public DbSet<Product> Products => Set<Product>();
	public DbSet<File> Files => Set<File>();
	public DbSet<Contact> Contacts => Set<Contact>();
	public DbSet<Page> Pages => Set<Page>();
	public DbSet<User> Users => Set<User>();
	public DbSet<Document> Documents => Set<Document>();

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
	}
}