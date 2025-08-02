using AutoMapper;
using Karya.Application.Features.File.Dto;
using Karya.Application.Features.Page.Dto;
using Karya.Application.Features.Product.Dto;
using Karya.Domain.Entities;
using File = Karya.Domain.Entities.File;

namespace Karya.Application.Mappings;

public class GeneralMapping : Profile
{
	public GeneralMapping()
	{
		CreateMap<Product, ProductDto>()
			.ForMember(dest => dest.Titles, opt => opt.MapFrom(src => src.Titles))
			.ForMember(dest => dest.Subtitles, opt => opt.MapFrom(src => src.Subtitles))
			.ForMember(dest => dest.Descriptions, opt => opt.MapFrom(src => src.Descriptions))
			.ForMember(dest => dest.ListTitles, opt => opt.MapFrom(src => src.ListTitles))
			.ForMember(dest => dest.ListItems, opt => opt.MapFrom(src => src.ListItems))
			.ForMember(dest => dest.Urls, opt => opt.MapFrom(src => src.Urls))
			.ForMember(dest => dest.BannerImageUrl, opt => opt.MapFrom(src => src.BannerImageUrl))
			.ForMember(dest => dest.ProductImageId, opt => opt.MapFrom(src => src.ProductImageId))
			.ForMember(dest => dest.DocumentImageIds, opt => opt.MapFrom(src => src.DocumentImageIds))
			.ForMember(dest => dest.ProductDetailImageIds, opt => opt.MapFrom(src => src.ProductDetailImageIds))
			.ForMember(dest => dest.FileIds, opt => opt.MapFrom(src => src.FileIds))
			.ForMember(dest => dest.Files, opt => opt.Ignore())
			.ForMember(dest => dest.ProductImage, opt => opt.Ignore())
			.ForMember(dest => dest.DocumentImages, opt => opt.Ignore())
			.ForMember(dest => dest.ProductImages, opt => opt.Ignore());

		CreateMap<ProductDto, Product>()
			.ForMember(dest => dest.Titles, opt => opt.MapFrom(src => src.Titles))
			.ForMember(dest => dest.Subtitles, opt => opt.MapFrom(src => src.Subtitles))
			.ForMember(dest => dest.Descriptions, opt => opt.MapFrom(src => src.Descriptions))
			.ForMember(dest => dest.ListTitles, opt => opt.MapFrom(src => src.ListTitles))
			.ForMember(dest => dest.ListItems, opt => opt.MapFrom(src => src.ListItems))
			.ForMember(dest => dest.Urls, opt => opt.MapFrom(src => src.Urls))
			.ForMember(dest => dest.BannerImageUrl, opt => opt.MapFrom(src => src.BannerImageUrl))
			.ForMember(dest => dest.ProductImageId, opt => opt.MapFrom(src => src.ProductImageId))
			.ForMember(dest => dest.DocumentImageIds, opt => opt.MapFrom(src => src.DocumentImageIds))
			.ForMember(dest => dest.ProductDetailImageIds, opt => opt.MapFrom(src => src.ProductDetailImageIds))
			.ForMember(dest => dest.FileIds, opt => opt.MapFrom(src => src.FileIds));

		CreateMap<CreateProductDto, Product>();
		CreateMap<UpdateProductDto, Product>();

		CreateMap<File, FileDto>().ReverseMap();
		CreateMap<CreateFileDto, File>();
		CreateMap<UpdateFileDto, File>();

		CreateMap<Page, PageDto>()
			.ForMember(dest => dest.Titles, opt => opt.MapFrom(src => src.Titles))
			.ForMember(dest => dest.Descriptions, opt => opt.MapFrom(src => src.Descriptions))
			.ForMember(dest => dest.ListTitles, opt => opt.MapFrom(src => src.ListTitles))
			.ForMember(dest => dest.Urls, opt => opt.MapFrom(src => src.Urls))
			.ReverseMap();
		CreateMap<CreatePageDto, Page>()
			.ForMember(dest => dest.FileIds, opt => opt.MapFrom(src => src.FileIds))
			.ForMember(dest => dest.ProductIds, opt => opt.MapFrom(src => src.ProductIds));
		CreateMap<UpdatePageDto, Page>()
			.ForMember(dest => dest.FileIds, opt => opt.MapFrom(src => src.FileIds))
			.ForMember(dest => dest.ProductIds, opt => opt.MapFrom(src => src.ProductIds));
		CreateMap<UpdatePageProductOrderDto, Page>()
			.ForMember(dest => dest.ProductIds, opt => opt.MapFrom(src => src.ProductIds));
	}
}