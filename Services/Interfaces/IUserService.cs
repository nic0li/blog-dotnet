using Blog.DTOs.User;

namespace Blog.Services.Interfaces;

public interface IUserService : ICrudService<
    UserResponse,
    UserProfileResponse,
    UserCreateRequest>
{
    Task<IEnumerable<UserProfileResponse>> GetAllAsync();

    Task<UserResponse> GetMeAsync();

    Task<UserResponse> UpdateMeAsync(UserUpdateRequest request);

    Task DeleteMeAsync();

    Task UpdatePasswordAsync(UserPasswordUpdateRequest request);

    Task<UserResponse> ToggleRoleAsync(long id);
}