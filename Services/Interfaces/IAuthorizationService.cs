using Blog.Entities;

namespace Blog.Services.Interfaces;

public interface IAuthorizationService
{
    Task<User> GetAuthenticatedUserAsync();

    Task<bool> IsOwnerAsync(User resourceOwner);

    Task<bool> IsAdminAsync();

    Task ValidateOwnerAsync(User resourceOwner);

    Task ValidateAdminAsync();

    Task ValidateOwnerOrAdminAsync(User resourceOwner);
}