using Blog.DTOs.User;
using Blog.Entities;
using Blog.Mappers;
using Blog.Repositories;
using Blog.Security.Interfaces;
using Blog.Services.Interfaces;

namespace Blog.Services;

public class UserService :
    CrudService<
        User,
        UserResponse,
        UserViewResponse,
        UserCreateRequest,
        UserUpdateRequest>,
    IUserService
{
    private readonly IUserRepository _repository;
    private readonly IPasswordService _passwordService;
    private readonly IAuthenticationService _authenticationService;

    public UserService(
        IUserRepository repository,
        IPasswordService passwordService,
        IAuthenticationService authenticationService)
        : base(repository)
    {
        _repository = repository;
        _passwordService = passwordService;
        _authenticationService = authenticationService;
    }

    public override async Task<UserResponse> CreateAsync(
        UserCreateRequest request)
    {
        var existingUser =
            await _repository.GetByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            throw new InvalidOperationException(
                "Email already registered");
        }

        var user = UserMapper.CreateEntity(request);

        user.Role = Enums.UserRole.User;
        user.Password = _passwordService.Hash(user.Password);

        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();

        return UserMapper.ToResponse(user);
    }

    public override async Task<UserResponse> UpdateAsync(
        long id,
        UserUpdateRequest request)
    {
        var user = await GetEntityByIdAsync(id);

        if (request.Email is not null)
        {
            var existingUser =
                await _repository.GetByEmailAsync(request.Email);

            if (existingUser is not null &&
                existingUser.Id != user.Id)
            {
                throw new InvalidOperationException(
                    "Email already registered");
            }
        }

        UserMapper.UpdateEntity(user, request);

        _repository.Update(user);

        await _repository.SaveChangesAsync();

        return UserMapper.ToResponse(user);
    }

    public override async Task DeleteAsync(long id)
    {
        var user = await GetEntityByIdAsync(id);

        _repository.Delete(user);

        await _repository.SaveChangesAsync();
    }

    public override async Task<UserViewResponse> GetByIdAsync(
        long id)
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
        var user = await _authenticationService
                .GetAuthenticatedUserAsync();

        return UserMapper.ToResponse(user);
    }

    public async Task<UserResponse> UpdateMeAsync(
        UserUpdateRequest request)
    {
        var user =
            await _authenticationService
                .GetAuthenticatedUserAsync();

        if (request.Email is not null)
        {
            var existingUser =
                await _repository.GetByEmailAsync(request.Email);

            if (existingUser is not null &&
                existingUser.Id != user.Id)
            {
                throw new InvalidOperationException(
                    "Email already registered");
            }
        }

        UserMapper.UpdateEntity(user, request);

        _repository.Update(user);

        await _repository.SaveChangesAsync();

        return UserMapper.ToResponse(user);
    }

    public async Task DeleteMeAsync()
    {
        var user =
            await _authenticationService
                .GetAuthenticatedUserAsync();

        _repository.Delete(user);

        await _repository.SaveChangesAsync();
    }
}