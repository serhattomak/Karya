using Karya.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = Karya.Domain.Entities.File;

namespace Karya.Persistence.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
	public void Configure(EntityTypeBuilder<Document> builder)
	{
		builder.ToTable("Documents");
		builder.HasKey(d => d.Id);

		builder.Property(d => d.Name)
			.IsRequired()
			.HasMaxLength(200);

		builder.Property(d => d.Slug)
			.IsRequired()
			.HasMaxLength(250);

		builder.Property(d => d.Description)
			.HasMaxLength(1000);

		builder.Property(d => d.Url)
			.HasMaxLength(500);

		builder.Property(d => d.PreviewImageUrl)
			.HasMaxLength(500);

		builder.Property(d => d.Category)
			.HasMaxLength(100);

		builder.Property(d => d.MimeType)
			.HasMaxLength(100);

		builder.HasIndex(d => d.Name)
			.IsUnique()
			.HasDatabaseName("IX_Documents_Name");

		builder.HasIndex(d => d.Slug)
			.IsUnique()
			.HasDatabaseName("IX_Documents_Slug");

		builder.HasOne<File>()
			.WithMany()
			.HasForeignKey(d => d.FileId)
			.OnDelete(DeleteBehavior.NoAction);

		builder.HasOne<File>()
			.WithMany()
			.HasForeignKey(d => d.PreviewImageFileId)
			.OnDelete(DeleteBehavior.NoAction);
	}
}