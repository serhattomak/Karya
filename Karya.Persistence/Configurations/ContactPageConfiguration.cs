using Karya.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Karya.Persistence.Configurations;

public class ContactPageConfiguration : IEntityTypeConfiguration<ContactPage>
{
	public void Configure(EntityTypeBuilder<ContactPage> builder)
	{
		builder.ToTable("ContactPages");
		builder.HasKey(p => p.Id);

		builder.Property(p => p.Title)
			.IsRequired()
			.HasMaxLength(200);

		builder.Property(p => p.Description)
			.HasMaxLength(1000);

		builder.Property(p => p.BackgroundImageUrl)
			.HasMaxLength(1000);

		builder.Property(p => p.PhoneNumbers)
			.HasConversion(
				v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
				v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>())
			.HasColumnType("TEXT");

		builder.Property(p => p.Emails)
			.HasConversion(
				v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
				v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>())
			.HasColumnType("TEXT");

		builder.Property(p => p.Addresses)
			.HasConversion(
				v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
				v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>())
			.HasColumnType("TEXT");

		builder.Property(p => p.SocialMediaLinks)
			.HasConversion(
				v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
				v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>())
			.HasColumnType("TEXT");
	}
}