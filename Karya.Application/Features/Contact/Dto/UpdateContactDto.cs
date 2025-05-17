namespace Karya.Application.Features.Contact.Dto;

public record UpdateContactDto(
	Guid Id,
	string FirstName,
	string LastName,
	string Email,
	string Phone,
	string Sector,
	string Message);