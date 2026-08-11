using Blog.DTOs.Category;
using Blog.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

[ApiController]
[Route("categories")]
[Authorize]
public class CategoryController(ICategoryService service) : ControllerBase
{
    private readonly ICategoryService _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAll([FromQuery] string? name)
    {
        var response = await _service.GetAllAsync(name);

        return Ok(response);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CategoryResponse>> GetById(long id)
    {
        var response = await _service.GetByIdAsync(id);

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create(CategoryRequest request)
    {
        var response = await _service.CreateAsync(request);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPatch("{id:long}")]
    public async Task<ActionResult<CategoryResponse>> Update(long id, CategoryRequest request)
    {
        var response = await _service.UpdateAsync(id, request);

        return Ok(response);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _service.DeleteAsync(id);

        return NoContent();
    }
}