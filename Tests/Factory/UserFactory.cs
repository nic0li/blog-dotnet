using Blog.DTOs.User;
using Blog.Entities;
using Blog.Enums;

namespace Blog.Tests.Factory;

public static class UserFactory
{
    public static User User()
    {
        return User(1L,
            "maria@email.com",
            "Maria",
            UserRole.User);
    }

    public static User Admin()
    {
        return User(2L,
            "ana@email.com",
            "Ana",
            UserRole.Admin);
    }

    public static UserCreateRequest CreateRequest()
    {
        return new UserCreateRequest(
            "maria@email.com",
            "123456",
            "Maria");
    }

    public static UserUpdateRequest UpdateRequest()
    {
        return UserUpdateRequest("mariasilva@email.com");
    }

    public static UserUpdateRequest UpdateRequestSameEmail()
    {
        return UserUpdateRequest("maria@email.com");
    }

    public static UserUpdateRequest UpdateRequestNullEmail()
    {
        return UserUpdateRequest(null);
    }

    public static UserUpdateRequest UpdateRequestEmptyEmail()
    {
        return UserUpdateRequest(string.Empty);
    }

    public static UserUpdateRequest UpdateRequestWithoutEmail()
    {
        return UserUpdateRequest();
    }

    public static UserResponse Response()
    {
        return UserResponse(
            "maria@email.com",
            "Maria",
            null);
    }

    public static UserResponse UpdatedResponse()
    {
        return UserResponse(
            "mariasilva@email.com",
            "Maria Silva",
            "dev");
    }

    public static UserResponse UpdatedResponseSameEmail()
    {
        return UserResponse(
            "maria@email.com",
            "Maria Silva",
            "dev");
    }

    public static UserViewResponse ViewResponse()
    {
        return new UserViewResponse(1L,
            "Maria",
            null,
            null);
    }

    private static User User(
        long id,
        string email,
        string name,
        UserRole role)
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

    private static UserUpdateRequest UserUpdateRequest(String? email)
    {
        UserUpdateRequest request = UserUpdateRequest();
        request.Email = email;
        return request;
    }

    private static UserUpdateRequest UserUpdateRequest()
    {
        return new UserUpdateRequest
        {
            Name = "Maria Silva",
            Photo = null,
            Bio = "dev"
        };
    }

    private static UserResponse UserResponse(
        string email,
        string? name,
        string? bio)
    {
        return new UserResponse(
            1L,
            email,
            name,
            null,
            bio,
            UserRole.User
        );
    }
}