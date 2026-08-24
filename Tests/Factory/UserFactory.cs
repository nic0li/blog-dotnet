using Blog.DTOs.User;
using Blog.Entities;
using Blog.Enums;

namespace Blog.Tests.Factory;

public static class UserFactory
{
    public static User User()
    {
        return User(1L, "maria@email.com", "Maria", UserRole.User);
    }

    public static User Admin()
    {
        return User(2L, "ana@email.com", "Ana", UserRole.Admin);
    }

    public static UserCreateRequest CreateRequest()
    {
        return new UserCreateRequest("maria@email.com", "123456", "Maria");
    }

    public static UserUpdateRequest UpdateRequest()
    {
        return new UserUpdateRequest
        {
            Name = "Maria Silva",
            Photo = "photo.jpg",
            Bio = "dev"
        };
    }

    public static UserUpdateRequest UpdateRequest(string? email)
    {
        var request = UpdateRequest();
        request.Email = email;
        return request;
    }

    public static UserUpdateRequest UpdateRequestWithBlankFields()
    {
        return new UserUpdateRequest
        {
            Name = "   ",
            Photo = "   ",
            Bio = "   "
        };
    }

    public static UserUpdateRequest UpdateRequestWithNullFields()
    {
        return new UserUpdateRequest
        {
            Email = "mariasilva@email.com"
        };
    }

    public static UserResponse Response()
    {
        return UserResponse("maria@email.com", "Maria", null, null);
    }

    public static UserResponse UpdatedResponse()
    {
        return UserResponse("mariasilva@email.com", "Maria Silva", "photo.jpg", "dev");
    }

    public static UserResponse UpdatedResponseSameEmail()
    {
        return UserResponse("maria@email.com", "Maria Silva", "photo.jpg", "dev");
    }

    public static UserResponse UpdatedResponseWithBlankFields()
    {
        return UserResponse("maria@email.com", null, null, null);
    }

    public static UserResponse UpdatedResponseWithNullFields()
    {
        return UserResponse("mariasilva@email.com", "Maria", null, null);
    }

    public static UserProfileResponse ProfileResponse()
    {
        return new UserProfileResponse(1L, "Maria", null, null);
    }

    private static User User(long id, string email, string name, UserRole role)
    {
        return new User
        {
            Id = id,
            Email = email,
            Password = "123456",
            Name = name,
            Role = role
        };
    }

    private static UserResponse UserResponse(string email, string? name, string? photo, string? bio)
    {
        return new UserResponse(1L, email, name, photo, bio, UserRole.User);
    }
}