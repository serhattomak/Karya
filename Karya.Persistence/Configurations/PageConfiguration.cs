using Karya.Domain.Entities;
using Karya.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Karya.Persistence.Configurations;

public class PageConfiguration : IEntityTypeConfiguration<Page>
{
	public void Configure(EntityTypeBuilder<Page> builder)
	{
		builder.ToTable("Pages");
		builder.HasKey(p => p.Id);

		builder.Property(p => p.PageType)
			.IsRequired();

		builder.Property(p => p.Name)
			.IsRequired()
			.HasMaxLength(200);

		builder.Property(p => p.Slug)
			.IsRequired()
			.HasMaxLength(250);

		builder.HasIndex(p => p.Name)
			.IsUnique()
			.HasDatabaseName("IX_Pages_Name");

		builder.HasIndex(p => p.Slug)
			.IsUnique()
			.HasDatabaseName("IX_Pages_Slug");

		builder.Property(p => p.BackgroundImageUrl)
			.HasMaxLength(500)
			.HasColumnType("varchar(500)");

		builder.Property(p => p.BannerImageUrl)
			.HasMaxLength(500)
			.HasColumnType("varchar(500)");

		builder.Property(p => p.MainImageUrl)
			.HasMaxLength(500)
			.HasColumnType("varchar(500)");

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

		builder.Property(p => p.FileIds)
			.HasConversion(JsonConversionHelper.GetGuidListConverter())
			.HasColumnType("text")
			.Metadata.SetValueComparer(JsonConversionHelper.GetGuidListComparer());

		builder.Property(p => p.ProductIds)
			.HasConversion(JsonConversionHelper.GetGuidListConverter())
			.HasColumnType("text")
			.Metadata.SetValueComparer(JsonConversionHelper.GetGuidListComparer());

		builder.Property(p => p.AdditionalFields)
			.HasConversion(JsonConversionHelper.GetListConverter<string>())
			.HasColumnType("text")
			.Metadata.SetValueComparer(JsonConversionHelper.GetListComparer<string>());

		builder.Property(p => p.DocumentIds)
			.HasConversion(JsonConversionHelper.GetGuidListConverter())
			.HasColumnType("text")
			.Metadata.SetValueComparer(JsonConversionHelper.GetGuidListComparer());
	}
}