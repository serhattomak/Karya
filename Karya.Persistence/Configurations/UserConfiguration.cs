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
			.HasDefaultValueSql("NEWID()");

		builder.Property(u => u.Username)
			.IsRequired()
			.HasMaxLength(50)
			.HasColumnType("nvarchar(50)");

		builder.Property(u => u.PasswordHash)
			.IsRequired()
			.HasMaxLength(255)
			.HasColumnType("nvarchar(255)");

		builder.Property(u => u.Role)
			.IsRequired()
			.HasDefaultValue(UserRoles.User);

		builder.HasIndex(u => u.Username)
			.IsUnique()
			.HasDatabaseName("IX_Users_Username");

		// Base entity properties
		builder.Property(u => u.CreatedDate)
			.IsRequired()
			.HasDefaultValueSql("GETUTCDATE()");

		builder.Property(u => u.ModifiedDate)
			.HasColumnType("datetime2");

		// Sentinel value ile enum configuration
		builder.Property(u => u.Status)
			.IsRequired()
			.HasDefaultValue(BaseStatuses.Active)
			.HasSentinel(BaseStatuses.None);
	}
}