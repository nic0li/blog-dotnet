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
    IAuthorizationService authorizationService) : CrudService<
    User,
    UserResponse,
    UserViewResponse,
    UserCreateRequest,
    UserUpdateRequest>(repository),
    IUserService
{
    private readonly IUserRepository _repository = repository;
    private readonly IPasswordService _passwordService = passwordService;
    private readonly IAuthorizationService _authorizationService = authorizationService;

    public override async Task<UserResponse> CreateAsync(UserCreateRequest request)
    {
        await ValidateEmailAvailabilityAsync(request.Email, null);

        var user = UserMapper.CreateEntity(request);

        user.Role = Enums.UserRole.User;
        user.Password = _passwordService.Hash(user.Password);

        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();

        return UserMapper.ToResponse(user);
    }

    public override async Task<UserResponse> UpdateAsync(long id, UserUpdateRequest request)
    {
        var user = await GetEntityByIdAsync(id);

        await _authorizationService.ValidateOwnerOrAdminAsync(user);

        return await UpdateUserResponseAsync(user, request);
    }

    public override async Task DeleteAsync(long id)
    {
        var user = await GetEntityByIdAsync(id);

        await _authorizationService.ValidateOwnerOrAdminAsync(user);

        await DeleteUserAsync(user);
    }

    public override async Task<UserViewResponse> GetByIdAsync(long id)
    {
        var user = await GetEntityByIdAsync(id);

        return UserMapper.ToViewResponse(user);
    }

    public async Task<IEnumerable<UserViewResponse>> GetAllAsync()
    {
        var users = await _repository.GetAllAsync();

        return users.Select(UserMapper.ToViewResponse);
    }

    public async Task<UserResponse> GetMeAsync()
    {
        var user = await _authorizationService.GetAuthenticatedUserAsync();

        return UserMapper.ToResponse(user);
    }

    public async Task<UserResponse> UpdateMeAsync(UserUpdateRequest request)
    {
        var user = await _authorizationService.GetAuthenticatedUserAsync();

        return await UpdateUserResponseAsync(user, request);
    }

    public async Task DeleteMeAsync()
    {
        var user = await _authorizationService.GetAuthenticatedUserAsync();

        await DeleteUserAsync(user);
    }

    public async Task UpdatePasswordAsync(UserPasswordUpdateRequest request)
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

    public async Task<UserResponse> ToggleRoleAsync(long id)
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

    private async Task<UserResponse> UpdateUserResponseAsync(User user, UserUpdateRequest request)
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

    private async Task DeleteUserAsync(User user)
    {
        _repository.Delete(user);

        await _repository.SaveChangesAsync();
    }

    private async Task ValidateEmailAvailabilityAsync(string email, long? userId)
    {
        var userByEmail = await _repository.GetByEmailAsync(email);

        var emailAlreadyExists = userByEmail is not null;

        var emailBelongsToAnotherUser = emailAlreadyExists && userByEmail!.Id != userId;

        if (emailAlreadyExists && emailBelongsToAnotherUser)
        {
            throw new BadRequestException("Email already registered");
        }
    }
}