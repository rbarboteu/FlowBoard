using FlowBoard.Application.DTOs.Projects;
using FlowBoard.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly ProjectService _projectService;

    public ProjectsController(ProjectService projectService)
    {
        _projectService = projectService;
    }

    private Guid GetTenantId() =>
        Guid.Parse(User.Claims.First(c => c.Type == "tenantId").Value);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _projectService.GetAllAsync(GetTenantId());
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest request)
    {
        var result = await _projectService.CreateAsync(request, GetTenantId());
        return CreatedAtAction(nameof(GetAll), result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _projectService.DeleteAsync(id, GetTenantId());
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectRequest request)
    {
        var result = await _projectService.UpdateAsync(id, request, GetTenantId());
        if (result is null) return NotFound();
        return Ok(result);
    }
}