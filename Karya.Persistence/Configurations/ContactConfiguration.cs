using Karya.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Karya.Persistence.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
	public void Configure(EntityTypeBuilder<Contact> builder)
	{
		builder.ToTable("Contacts");
		builder.HasKey(c => c.Id);

		builder.Property(c => c.FirstName)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(c => c.LastName)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(c => c.Email)
			.HasMaxLength(200);

		builder.Property(c => c.Phone)
			.IsRequired()
			.HasMaxLength(15);

		builder.Property(c => c.Sector)
			.HasMaxLength(100);

		builder.Property(c => c.Message)
			.IsRequired()
			.HasMaxLength(2000);
	}
}