using Karya.Domain.Enums;

namespace Karya.Domain.Common;

public class BaseEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
	public DateTime? ModifiedDate { get; set; }
	public BaseStatuses Status { get; set; } = BaseStatuses.Active;
}