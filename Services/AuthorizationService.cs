using Blog.Entities;
using Blog.Enums;
using Blog.Exceptions;
using Blog.Services.Interfaces;

namespace Blog.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly IAuthenticationService _authenticationService;

    public AuthorizationService(
        IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<User> GetAuthenticatedUserAsync()
    {
        return await _authenticationService
            .GetAuthenticatedUserAsync();
    }

    public async Task<bool> IsOwnerAsync(User resourceOwner)
    {
        var authenticatedUser =
            await GetAuthenticatedUserAsync();

        return IsOwner(resourceOwner, authenticatedUser);
    }

    public async Task<bool> IsAdminAsync()
    {
        var authenticatedUser =
            await GetAuthenticatedUserAsync();

        return IsAdmin(authenticatedUser);
    }

    public async Task ValidateOwnerAsync(User resourceOwner)
    {
        if (!await IsOwnerAsync(resourceOwner))
        {
            throw new ForbiddenException(
                "You are not allowed to modify this resource");
        }
    }

    public async Task ValidateAdminAsync()
    {
        if (!await IsAdminAsync())
        {
            throw new ForbiddenException(
                "Administrator privileges required");
        }
    }

    public async Task ValidateOwnerOrAdminAsync(
        User resourceOwner)
    {
        var authenticatedUser =
            await GetAuthenticatedUserAsync();

        if (!IsOwner(resourceOwner, authenticatedUser) &&
            !IsAdmin(authenticatedUser))
        {
            throw new ForbiddenException(
                "You are not allowed to modify this resource");
        }
    }

    private static bool IsOwner(
        User resourceOwner,
        User authenticatedUser)
    {
        return resourceOwner.Id == authenticatedUser.Id;
    }

    private static bool IsAdmin(User authenticatedUser)
    {
        return authenticatedUser.Role == UserRole.Admin;
    }
}