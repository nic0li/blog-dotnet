using Blog.DTOs.User;
using Blog.Entities;
using Blog.Exceptions;
using Blog.Mappers;
using Blog.Repositories.Interfaces;
using Blog.Security.Interfaces;
using Blog.Services.Interfaces;

namespace Blog.Services;

public class UserService(
    IUserRepository repository,
    IPasswordService passwordService,
    IAuthorizationService authorizationService)
    : EntityService<User>(repository), IUserService
{
    private readonly IUserRepository _repository = repository;
    private readonly IPasswordService _passwordService = passwordService;
    private readonly IAuthorizationService _authorizationService = authorizationService;

    public async Task<UserResponse> CreateAsync(UserCreateRequest request)
    {
        await ValidateEmailAvailabilityAsync(request.Email, null);

        var user = UserMapper.CreateEntity(request);

        user.Role = Enums.UserRole.User;
        user.Password = _passwordService.Hash(user.Password);

        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();

        return UserMapper.ToResponse(user);
    }

    public async Task<UserProfileResponse> GetByIdAsync(long id)
    {
        var user = await GetEntityByIdAsync(id);

        return UserMapper.ToProfileResponse(user);
    }

    public async Task<IEnumerable<UserProfileResponse>> GetAllAsync()
    {
        var users = await _repository.GetAllAsync();

        return users.Select(UserMapper.ToProfileResponse);
    }

    public async Task<UserResponse> ToggleUserRoleAsync(long id)
    {
        await _authorizationService.ValidateAdminAsync();

        var authenticatedUser = await _authorizationService.GetAuthenticatedUserAsync();

        var user = await GetEntityByIdAsync(id);

        if (authenticatedUser.Id == user.Id)
        {
            throw new BadRequestException("You cannot change your own role");
        }

        user.Role = user.Role == Enums.UserRole.Admin
            ? Enums.UserRole.User
            : Enums.UserRole.Admin;

        _repository.Update(user);

        await _repository.SaveChangesAsync();

        return UserMapper.ToResponse(user);
    }

    public async Task DeleteUserAsync(long id)
    {
        var user = await GetEntityByIdAsync(id);

        await _authorizationService.ValidateOwnerOrAdminAsync(user);

        _repository.Delete(user);

        await _repository.SaveChangesAsync();
    }

    public async Task<UserResponse> GetAuthenticatedAsync()
    {
        var user = await _authorizationService.GetAuthenticatedUserAsync();

        return UserMapper.ToResponse(user);
    }

    public async Task<UserResponse> UpdateAuthenticatedAsync(UserUpdateRequest request)
    {
        var user = await _authorizationService.GetAuthenticatedUserAsync();

        return await UpdateUserAsync(request, user);
    }

    public async Task UpdateAuthenticatedPasswordAsync(UserPasswordUpdateRequest request)
    {
        var user = await _authorizationService.GetAuthenticatedUserAsync();

        var passwordIsValid = _passwordService.Verify(request.CurrentPassword, user.Password);

        if (!passwordIsValid)
        {
            throw new UnauthorizedAccessException("Invalid current password");
        }

        user.Password = _passwordService.Hash(request.NewPassword);

        _repository.Update(user);

        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAuthenticatedAsync()
    {
        var user = await _authorizationService.GetAuthenticatedUserAsync();

        _repository.Delete(user);

        await _repository.SaveChangesAsync();
    }

    private async Task<UserResponse> UpdateUserAsync(UserUpdateRequest request, User user)
    {
        if (request.EmailProvided
            && !string.IsNullOrEmpty(request.Email))
        {
            await ValidateEmailAvailabilityAsync(request.Email, user.Id);
        }

        UserMapper.UpdateEntity(user, request);

        _repository.Update(user);

        await _repository.SaveChangesAsync();

        return UserMapper.ToResponse(user);
    }

    private async Task ValidateEmailAvailabilityAsync(string email, long? userId)
    {
        var userByEmail = await _repository.GetByEmailAsync(email);

        bool emailAlreadyExists = userByEmail is not null;

        bool emailBelongsToAnotherUser = emailAlreadyExists && userByEmail!.Id != userId;

        if (emailAlreadyExists && emailBelongsToAnotherUser)
        {
            throw new BadRequestException("Email already registered");
        }
    }
}