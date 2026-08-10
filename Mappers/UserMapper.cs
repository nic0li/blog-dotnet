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
        if (request.Email is not null)
        {
            user.Email = request.Email;
        }

        if (request.Name is not null)
        {
            user.Name = request.Name;
        }

        if (request.Photo is not null)
        {
            user.Photo = request.Photo;
        }

        if (request.Bio is not null)
        {
            user.Bio = request.Bio;
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