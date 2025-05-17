namespace Karya.Application.Features.Contact.Dto;

public class ContactDto
{
	public Guid Id { get; set; }
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string Phone { get; set; } = string.Empty;
	public string Sector { get; set; } = string.Empty;
	public string Message { get; set; } = string.Empty;
}