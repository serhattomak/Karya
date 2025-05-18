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
			.ForMember(dest => dest.Descriptions, opt => opt.MapFrom(src => src.Descriptions))
			.ForMember(dest => dest.Urls, opt => opt.MapFrom(src => src.Urls))
			.ForMember(dest => dest.FileIds, opt => opt.MapFrom(src => src.FileIds))
			.ReverseMap();
		CreateMap<CreateProductDto, Product>();
		CreateMap<UpdateProductDto, Product>();

		CreateMap<File, FileDto>().ReverseMap();
		CreateMap<CreateFileDto, File>();
		CreateMap<UpdateFileDto, File>();

		CreateMap<Page, PageDto>()
			.ForMember(dest => dest.Titles, opt => opt.MapFrom(src => src.Titles))
			.ForMember(dest => dest.Descriptions, opt => opt.MapFrom(src => src.Descriptions))
			.ForMember(dest => dest.Urls, opt => opt.MapFrom(src => src.Urls))
			.ReverseMap();
		CreateMap<CreatePageDto, Page>()
			.ForMember(dest => dest.FileIds, opt => opt.MapFrom(src => src.FileIds))
			.ForMember(dest => dest.ProductIds, opt => opt.MapFrom(src => src.ProductIds));
		CreateMap<UpdatePageDto, Page>()
			.ForMember(dest => dest.FileIds, opt => opt.MapFrom(src => src.FileIds))
			.ForMember(dest => dest.ProductIds, opt => opt.MapFrom(src => src.ProductIds));
	}
}