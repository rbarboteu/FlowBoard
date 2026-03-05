using FlowBoard.Application.DTOs.Tasks;
using FlowBoard.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly TaskService _taskService;

    public TasksController(TaskService taskService)
    {
        _taskService = taskService;
    }

    private Guid GetTenantId() =>
        Guid.Parse(User.Claims.First(c => c.Type == "tenantId").Value);

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetByProject(Guid projectId)
    {
        var result = await _taskService.GetAllByProjectAsync(projectId, GetTenantId());
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
    {
        var result = await _taskService.CreateAsync(request, GetTenantId());
        return CreatedAtAction(nameof(GetByProject), new { projectId = result.ProjectId }, result);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
    {
        var success = await _taskService.UpdateStatusAsync(id, request.Status, GetTenantId());
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _taskService.DeleteAsync(id, GetTenantId());
        if (!success) return NotFound();
        return NoContent();
    }
}