using Karya.Domain.Common;

namespace Karya.Domain.Entities;

public class ContactPage : BaseEntity
{
	public string Title { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string BackgroundImageUrl { get; set; } = string.Empty;
	public List<string> PhoneNumbers { get; set; } = [];
	public List<string> Emails { get; set; } = [];
	public List<string> Addresses { get; set; } = [];
	public List<string> SocialMediaLinks { get; set; } = [];
}