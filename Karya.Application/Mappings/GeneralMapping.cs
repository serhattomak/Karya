using AutoMapper;
using Karya.Application.Features.AboutPage.Dto;
using Karya.Application.Features.Contact.Dto;
using Karya.Application.Features.ContactPage.Dto;
using Karya.Application.Features.File.Dto;
using Karya.Application.Features.HomePage.Dto;
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

		CreateMap<AboutPage, AboutPageDto>().ReverseMap();
		CreateMap<CreateAboutPageDto, AboutPage>();
		CreateMap<UpdateAboutPageDto, AboutPage>();

		CreateMap<Contact, ContactDto>().ReverseMap();
		CreateMap<CreateContactDto, Contact>();
		CreateMap<UpdateContactDto, Contact>();

		CreateMap<ContactPage, ContactPageDto>().ReverseMap();
		CreateMap<CreateContactPageDto, ContactPage>();
		CreateMap<UpdateContactPageDto, ContactPage>();

		CreateMap<HomePage, HomePageDto>().ReverseMap();
		CreateMap<CreateHomePageDto, HomePage>();
		CreateMap<UpdateHomePageDto, HomePage>()
			.ForMember(dest => dest.FileIds, opt => opt.MapFrom(src => src.FileIds))
			.ForMember(dest => dest.ProductIds, opt => opt.MapFrom(src => src.ProductIds));

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