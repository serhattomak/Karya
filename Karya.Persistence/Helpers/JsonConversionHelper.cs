using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Karya.Persistence.Helpers;

public static class JsonConversionHelper
{
	private static readonly JsonSerializerOptions DefaultJsonOptions = new()
	{
		PropertyNamingPolicy = null,
		WriteIndented = false,
		DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
	};

	public static ValueConverter<List<T>?, string> GetListConverter<T>()
	{
		return new ValueConverter<List<T>?, string>(
			v => v == null ? null : JsonSerializer.Serialize(v, DefaultJsonOptions),
			v => v == null ? null : JsonSerializer.Deserialize<List<T>>(v, DefaultJsonOptions));
	}

	public static ValueComparer<List<T>?> GetListComparer<T>()
	{
		return new ValueComparer<List<T>?>(
			(c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
			c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v == null ? 0 : v.GetHashCode())),
			c => c == null ? null : c.ToList());
	}

	public static ValueConverter<List<Guid>?, string> GetGuidListConverter()
	{
		return new ValueConverter<List<Guid>?, string>(
			v => v == null ? null : JsonSerializer.Serialize(v, DefaultJsonOptions),
			v => v == null ? null : JsonSerializer.Deserialize<List<Guid>>(v, DefaultJsonOptions));
	}

	public static ValueComparer<List<Guid>?> GetGuidListComparer()
	{
		return new ValueComparer<List<Guid>?>(
			(c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
			c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
			c => c == null ? null : c.ToList());
	}
}