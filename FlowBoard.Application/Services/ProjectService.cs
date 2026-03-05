using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowBoard.Application.DTOs.Projects;
using FlowBoard.Application.Interfaces;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Application.Services;

public class ProjectService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<List<ProjectResponse>> GetAllAsync(Guid tenantId)
    {
        var projects = await _projectRepository.GetAllByTenantAsync(tenantId);

        return projects.Select(p => new ProjectResponse
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description
        }).ToList();
    }

    public async Task<ProjectResponse> CreateAsync(CreateProjectRequest request, Guid tenantId)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = request.Name,
            Description = request.Description
        };

        await _projectRepository.AddAsync(project);

        return new ProjectResponse
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description
        };
    }

    public async Task<bool> DeleteAsync(Guid id, Guid tenantId)
    {
        var project = await _projectRepository.GetByIdAsync(id, tenantId);
        if (project is null) return false;

        await _projectRepository.DeleteAsync(project);
        return true;
    }

    public async Task<ProjectResponse?> UpdateAsync(Guid id, UpdateProjectRequest request, Guid tenantId)
    {
        var project = await _projectRepository.GetByIdAsync(id, tenantId);
        if (project is null) return null;

        project.Name = request.Name;
        project.Description = request.Description;

        await _projectRepository.UpdateAsync(project);

        return new ProjectResponse
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description
        };
    }

}