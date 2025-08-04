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

		builder.Property(p => p.Slug)
			.IsRequired()
			.HasMaxLength(250);

		builder.HasIndex(p => p.Name)
			.IsUnique()
			.HasDatabaseName("IX_Products_Name");

		builder.HasIndex(p => p.Slug)
			.IsUnique()
			.HasDatabaseName("IX_Products_Slug");

		// String property
		builder.Property(p => p.BannerImageUrl)
			.HasMaxLength(500)
			.HasColumnType("nvarchar(500)");

		// Guid property for single product image
		builder.Property(p => p.ProductImageId)
			.HasColumnType("uniqueidentifier");

		// String List Properties
		builder.Property(p => p.Titles)
			.HasConversion(
				v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
				v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
			.HasColumnType("NVARCHAR(MAX)")
			.Metadata.SetValueComparer(new ValueComparer<List<string>>(
				(c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
				c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
				c => c.ToList()));

		builder.Property(p => p.Subtitles)
			.HasConversion(
				v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) : null,
				v => v != null ? JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) : null)
			.HasColumnType("NVARCHAR(MAX)")
			.Metadata.SetValueComparer(new ValueComparer<List<string>?>(
				(c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
				c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
				c => c == null ? null : c.ToList()));

		builder.Property(p => p.Descriptions)
			.HasConversion(
				v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) : null,
				v => v != null ? JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) : null)
			.HasColumnType("NVARCHAR(MAX)")
			.Metadata.SetValueComparer(new ValueComparer<List<string>?>(
				(c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
				c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
				c => c == null ? null : c.ToList()));

		builder.Property(p => p.ListTitles)
			.HasConversion(
				v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) : null,
				v => v != null ? JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) : null)
			.HasColumnType("NVARCHAR(MAX)")
			.Metadata.SetValueComparer(new ValueComparer<List<string>?>(
				(c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
				c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
				c => c == null ? null : c.ToList()));

		builder.Property(p => p.ListItems)
			.HasConversion(
				v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) : null,
				v => v != null ? JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) : null)
			.HasColumnType("NVARCHAR(MAX)")
			.Metadata.SetValueComparer(new ValueComparer<List<string>?>(
				(c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
				c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
				c => c == null ? null : c.ToList()));

		builder.Property(p => p.Urls)
			.HasConversion(
				v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) : null,
				v => v != null ? JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) : null)
			.HasColumnType("NVARCHAR(MAX)")
			.Metadata.SetValueComparer(new ValueComparer<List<string>?>(
				(c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
				c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
				c => c == null ? null : c.ToList()));

		// Guid List Properties
		builder.Property(p => p.DocumentImageIds)
			.HasConversion(
				v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) : null,
				v => v != null ? JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null) : null)
			.HasColumnType("NVARCHAR(MAX)")
			.Metadata.SetValueComparer(new ValueComparer<List<Guid>?>(
				(c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
				c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
				c => c == null ? null : c.ToList()));

		builder.Property(p => p.ProductDetailImageIds)
			.HasConversion(
				v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) : null,
				v => v != null ? JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null) : null)
			.HasColumnType("NVARCHAR(MAX)")
			.Metadata.SetValueComparer(new ValueComparer<List<Guid>?>(
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

		builder.Property(p => p.DocumentIds)
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