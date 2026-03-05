using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowBoard.Application.DTOs.Tasks;
using FlowBoard.Application.Interfaces;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Enums;

namespace FlowBoard.Application.Services;

public class TaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<List<TaskResponse>> GetAllByProjectAsync(Guid projectId, Guid tenantId)
    {
        var tasks = await _taskRepository.GetAllByProjectAsync(projectId, tenantId);

        return tasks.Select(t => new TaskResponse
        {
            Id = t.Id,
            Title = t.Title,
            Status = t.Status.ToString(),
            ProjectId = t.ProjectId
        }).ToList();
    }

    public async Task<TaskResponse> CreateAsync(CreateTaskRequest request, Guid tenantId)
    {
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProjectId = request.ProjectId,
            Title = request.Title,
            Status = TaskItemStatus.Todo
        };

        await _taskRepository.AddAsync(task);

        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Status = task.Status.ToString(),
            ProjectId = task.ProjectId
        };
    }

    public async Task<bool> UpdateStatusAsync(Guid id, string newStatus, Guid tenantId)
    {
        var task = await _taskRepository.GetByIdAsync(id, tenantId);
        if (task is null) return false;

        task.Status = Enum.Parse<TaskItemStatus>(newStatus);
        await _taskRepository.UpdateAsync(task);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid tenantId)
    {
        var task = await _taskRepository.GetByIdAsync(id, tenantId);
        if (task is null) return false;

        await _taskRepository.DeleteAsync(task);
        return true;
    }
}