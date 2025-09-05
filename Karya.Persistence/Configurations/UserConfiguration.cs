using Karya.Domain.Entities;
using Karya.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Karya.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.ToTable("Users");

		builder.HasKey(u => u.Id);

		builder.Property(u => u.Id)
			.HasDefaultValueSql("gen_random_uuid()");

		builder.Property(u => u.Username)
			.IsRequired()
			.HasMaxLength(50)
			.HasColumnType("varchar(50)");

		builder.Property(u => u.PasswordHash)
			.IsRequired()
			.HasMaxLength(255)
			.HasColumnType("varchar(255)");

		builder.Property(u => u.Role)
			.IsRequired()
			.HasDefaultValue(UserRoles.User);

		builder.HasIndex(u => u.Username)
			.IsUnique()
			.HasDatabaseName("IX_Users_Username");

		// Base entity properties
		builder.Property(u => u.CreatedDate)
			.IsRequired()
			.HasDefaultValueSql("CURRENT_TIMESTAMP");

		builder.Property(u => u.ModifiedDate)
			.HasColumnType("timestamp with time zone");

		// Sentinel value ile enum configuration
		builder.Property(u => u.Status)
			.IsRequired()
			.HasDefaultValue(BaseStatuses.Active)
			.HasSentinel(BaseStatuses.None);
	}
}