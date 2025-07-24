using Karya.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Karya.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
	public void Configure(EntityTypeBuilder<Product> builder)
	{
		builder.ToTable("Products");
		builder.HasKey(p => p.Id);

		builder.Property(p => p.Name)
			.IsRequired()
			.HasMaxLength(200);

		builder.Property(p => p.Titles)
			.HasConversion(
				v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
				v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
			.HasColumnType("NVARCHAR(MAX)")
			.Metadata.SetValueComparer(new ValueComparer<List<string>>(
				(c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
				c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
				c => c.ToList()));

		builder.Property(p => p.Descriptions)
			.HasConversion(
				v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
				v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
			.HasColumnType("NVARCHAR(MAX)")
			.Metadata.SetValueComparer(new ValueComparer<List<string>>(
				(c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
				c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
				c => c.ToList()));

		builder.Property(p => p.Urls)
			.HasConversion(
				v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) : null,
				v => v != null ? JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) : null)
			.HasColumnType("NVARCHAR(MAX)")
			.Metadata.SetValueComparer(new ValueComparer<List<string>?>(
				(c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
				c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
				c => c == null ? null : c.ToList()));

		builder.Property(p => p.FileIds)
			.HasConversion(
				v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) : null,
				v => v != null ? JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null) : null)
			.HasColumnType("NVARCHAR(MAX)")
			.Metadata.SetValueComparer(new ValueComparer<List<Guid>?>(
				(c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
				c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
				c => c == null ? null : c.ToList()));
	}
}