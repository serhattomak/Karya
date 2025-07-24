namespace Karya.Application.Features.User.Services.Interfaces;

public interface ITokenService
{
	string GenerateToken(Domain.Entities.User user);
	bool ValidateToken(string token);
	Guid? GetUserIdFromToken(string token);
}