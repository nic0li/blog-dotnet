namespace Blog.Services.Interfaces;

public interface ICrudService<
    Response,
    ViewResponse,
    CreateRequest>
{
    Task<Response> CreateAsync(CreateRequest request);

    Task DeleteAsync(long id);

    Task<ViewResponse> GetByIdAsync(long id);
}