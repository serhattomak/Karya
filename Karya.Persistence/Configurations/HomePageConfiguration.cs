using Karya.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Karya.Persistence.Configurations;

public class HomePageConfiguration : IEntityTypeConfiguration<HomePage>
{
	public void Configure(EntityTypeBuilder<HomePage> builder)
	{
		builder.ToTable("HomePages");
		builder.HasKey(p => p.Id);

		builder.Property(p => p.Title)
			.IsRequired()
			.HasMaxLength(200);

		builder.Property(p => p.Description)
			.HasMaxLength(1000);

		builder.Property(p => p.BackgroundImageUrl)
			.HasMaxLength(1000);

		builder.Property(p => p.FileIds)
			.HasConversion(
				v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
				v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions)null) ?? new List<Guid>())
			.HasColumnType("TEXT");

		builder.Property(p => p.ProductIds)
			.HasConversion(
				v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
				v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions)null) ?? new List<Guid>())
			.HasColumnType("TEXT");
	}
}