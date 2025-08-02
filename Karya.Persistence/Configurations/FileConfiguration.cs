using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = Karya.Domain.Entities.File;

namespace Karya.Persistence.Configurations;

public class FileConfiguration : IEntityTypeConfiguration<File>
{
	public void Configure(EntityTypeBuilder<File> builder)
	{
		builder.ToTable("Files");
		builder.HasKey(f => f.Id);
		builder.Property(f => f.Name)
			.IsRequired()
			.HasMaxLength(200);
		builder.Property(f => f.Path)
			.IsRequired()
			.HasMaxLength(500);
		builder.Property(f => f.Size)
			.IsRequired();
		builder.Property(f => f.ContentType)
			.IsRequired()
			.HasMaxLength(50);
		builder.Property(f => f.Hash)
			.IsRequired()
			.HasMaxLength(64);

		builder.HasIndex(f => f.Hash)
			.IsUnique()
			.HasDatabaseName("IX_Files_Hash");
	}
}