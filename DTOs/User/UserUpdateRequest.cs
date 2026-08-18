using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Blog.DTOs.User;

public record UserUpdateRequest
{
    [EmailAddress(ErrorMessage = "Invalid email")]
    public string? Email
    {
        get;
        set
        {
            EmailProvided = true;
            field = value;
        }
    }

    public string? Name
    {
        get;
        set
        {
            NameProvided = true;
            field = value;
        }
    }

    public string? Photo
    {
        get;
        set
        {
            PhotoProvided = true;
            field = value;
        }
    }

    public string? Bio
    {
        get;
        set
        {
            BioProvided = true;
            field = value;
        }
    }

    [JsonIgnore]
    public bool EmailProvided { get; private set; }

    [JsonIgnore]
    public bool NameProvided { get; private set; }

    [JsonIgnore]
    public bool PhotoProvided { get; private set; }

    [JsonIgnore]
    public bool BioProvided { get; private set; }

}