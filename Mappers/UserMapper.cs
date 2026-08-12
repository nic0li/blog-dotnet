using Blog.DTOs.User;
using Blog.Entities;

namespace Blog.Mappers;

public static class UserMapper
{
    public static User CreateEntity(UserCreateRequest request)
    {
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = request.Password
        };

        return user;
    }

    public static void UpdateEntity(
        User user,
        UserUpdateRequest request)
    {
        if (request.EmailProvided && !string.IsNullOrWhiteSpace(request.Email))
        {
            user.Email = request.Email;
        }

        if (request.NameProvided)
        {
            user.Name = string.IsNullOrWhiteSpace(request.Name)
                ? null : request.Name;
        }

        if (request.PhotoProvided)
        {
            user.Photo = string.IsNullOrWhiteSpace(request.Photo)
                ? null : request.Photo;
        }

        if (request.BioProvided)
        {
            user.Bio = string.IsNullOrWhiteSpace(request.Bio)
                ? null : request.Bio;
        }
    }

    public static UserResponse ToResponse(User user)
    {
        return new UserResponse(
            user.Id,
            user.Email,
            user.Name,
            user.Photo,
            user.Bio,
            user.Role);
    }

    public static UserViewResponse ToViewResponse(User user)
    {
        return new UserViewResponse(
            user.Id,
            user.Name,
            user.Photo,
            user.Bio);
    }
}