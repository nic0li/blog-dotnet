namespace Blog.Services.Interfaces;

public interface ICrudService<
    Response,
    ViewResponse,
    CreateRequest,
    UpdateRequest>
{
    Task<Response> CreateAsync(CreateRequest request);

    Task<Response> UpdateAsync(long id, UpdateRequest request);

    Task DeleteAsync(long id);

    Task<ViewResponse> GetByIdAsync(long id);
}