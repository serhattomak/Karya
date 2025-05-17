using Karya.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Karya.Persistence.Configurations;

public class PageConfiguration : IEntityTypeConfiguration<Page>
{
	public void Configure(EntityTypeBuilder<Page> builder)
	{
		builder.ToTable("Pages");
		builder.HasKey(p => p.Id);

		builder.Property(p => p.Name)
			.IsRequired()
			.HasMaxLength(200);

		builder.Property(p => p.Titles)
			.HasConversion(
				v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
				v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>())
			.HasColumnType("TEXT");
		builder.Property(p => p.Descriptions)
			.HasConversion(
				v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
				v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>())
			.HasColumnType("TEXT");
		builder.Property(p => p.Urls)
			.HasConversion(
				v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
				v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>())
			.HasColumnType("TEXT");
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
		builder.Property(p => p.AdditionalFields)
			.HasConversion(
				v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
				v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>())
			.HasColumnType("TEXT");
	}
}