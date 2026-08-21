using Blog.DTOs.User;
using Blog.Entities;

namespace Blog.Services.Interfaces;

public interface IUserService : IEntityService<User>
{
    Task<UserResponse> CreateAsync(UserCreateRequest request);

    Task<UserProfileResponse> GetByIdAsync(long id);

    Task<IEnumerable<UserProfileResponse>> GetAllAsync();

    Task<UserResponse> ToggleUserRoleAsync(long id);

    Task DeleteUserAsync(long id);

    Task<UserResponse> GetAuthenticatedAsync();

    Task<UserResponse> UpdateAuthenticatedAsync(UserUpdateRequest request);

    Task UpdateAuthenticatedPasswordAsync(UserPasswordUpdateRequest request);

    Task DeleteAuthenticatedAsync();
}