namespace Karya.Application.Features.Contact.Dto;

public record CreateContactDto(string FirstName, string LastName, string Email, string Phone, string Sector, string Message);