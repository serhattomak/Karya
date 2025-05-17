namespace Karya.Application.Features.ContactPage.Dto;

public record UpdateContactPageDto(
	Guid Id,
	string Title,
	string Description,
	string BackgroundImageUrl,
	List<string> PhoneNumbers,
	List<string> Emails,
	List<string> Addresses,
	List<string> SocialMediaLinks
);