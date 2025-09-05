using Karya.Domain.Entities;
using Karya.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

		builder.Property(p => p.HomePageSubtitle)
			.HasMaxLength(500)
			.HasColumnType("varchar(500)");

		builder.HasIndex(p => p.Name)
			.IsUnique()
			.HasDatabaseName("IX_Products_Name");

		builder.HasIndex(p => p.Slug)
			.IsUnique()
			.HasDatabaseName("IX_Products_Slug");

		builder.Property(p => p.BannerImageUrl)
			.HasMaxLength(500)
			.HasColumnType("varchar(500)");

		builder.Property(p => p.MainImageUrl)
			.HasMaxLength(500)
			.HasColumnType("varchar(500)");

		builder.Property(p => p.ShowContact)
			.HasDefaultValue(false);

		builder.Property(p => p.ProductImageId)
			.HasColumnType("uuid");

		builder.Property(p => p.ProductMainImageId)
			.HasColumnType("uuid");

		builder.Property(p => p.Titles)
			.HasConversion(JsonConversionHelper.GetListConverter<string>())
			.HasColumnType("text")
			.Metadata.SetValueComparer(JsonConversionHelper.GetListComparer<string>());

		builder.Property(p => p.Subtitles)
			.HasConversion(JsonConversionHelper.GetListConverter<string>())
			.HasColumnType("text")
			.Metadata.SetValueComparer(JsonConversionHelper.GetListComparer<string>());

		builder.Property(p => p.Descriptions)
			.HasConversion(JsonConversionHelper.GetListConverter<string>())
			.HasColumnType("text")
			.Metadata.SetValueComparer(JsonConversionHelper.GetListComparer<string>());

		builder.Property(p => p.ListTitles)
			.HasConversion(JsonConversionHelper.GetListConverter<string>())
			.HasColumnType("text")
			.Metadata.SetValueComparer(JsonConversionHelper.GetListComparer<string>());

		builder.Property(p => p.ListItems)
			.HasConversion(JsonConversionHelper.GetListConverter<string>())
			.HasColumnType("text")
			.Metadata.SetValueComparer(JsonConversionHelper.GetListComparer<string>());

		builder.Property(p => p.Urls)
			.HasConversion(JsonConversionHelper.GetListConverter<string>())
			.HasColumnType("text")
			.Metadata.SetValueComparer(JsonConversionHelper.GetListComparer<string>());

		builder.Property(p => p.VideoTitles)
			.HasConversion(JsonConversionHelper.GetListConverter<string>())
			.HasColumnType("text")
			.Metadata.SetValueComparer(JsonConversionHelper.GetListComparer<string>());

		builder.Property(p => p.VideoUrls)
			.HasConversion(JsonConversionHelper.GetListConverter<string>())
			.HasColumnType("text")
			.Metadata.SetValueComparer(JsonConversionHelper.GetListComparer<string>());

		builder.Property(p => p.VideoDescriptions)
			.HasConversion(JsonConversionHelper.GetListConverter<string>())
			.HasColumnType("text")
			.Metadata.SetValueComparer(JsonConversionHelper.GetListComparer<string>());

		builder.Property(p => p.DocumentImageIds)
			.HasConversion(JsonConversionHelper.GetGuidListConverter())
			.HasColumnType("text")
			.Metadata.SetValueComparer(JsonConversionHelper.GetGuidListComparer());

		builder.Property(p => p.ProductDetailImageIds)
			.HasConversion(JsonConversionHelper.GetGuidListConverter())
			.HasColumnType("text")
			.Metadata.SetValueComparer(JsonConversionHelper.GetGuidListComparer());

		builder.Property(p => p.FileIds)
			.HasConversion(JsonConversionHelper.GetGuidListConverter())
			.HasColumnType("text")
			.Metadata.SetValueComparer(JsonConversionHelper.GetGuidListComparer());

		builder.Property(p => p.DocumentIds)
			.HasConversion(JsonConversionHelper.GetGuidListConverter())
			.HasColumnType("text")
			.Metadata.SetValueComparer(JsonConversionHelper.GetGuidListComparer());
	}
}