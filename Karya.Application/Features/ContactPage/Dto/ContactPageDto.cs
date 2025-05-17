namespace Karya.Application.Features.ContactPage.Dto;

public class ContactPageDto
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string BackgroundImageUrl { get; set; } = string.Empty;
	public List<string> PhoneNumbers { get; set; } = [];
	public List<string> Emails { get; set; } = [];
	public List<string> Addresses { get; set; } = [];
	public List<string> SocialMediaLinks { get; set; } = [];
}