using AutoMapper;
using Karya.Application.Common.DTOs;
using Karya.Infrastructure.Entities;

namespace Karya.Infrastructure.Mappings;

public class GeneralMapping : Profile
{
	public GeneralMapping()
	{
		CreateMap<RegisterDto, AppUser>();
		CreateMap<AppUser, AuthDto>();
	}
}