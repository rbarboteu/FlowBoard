using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowBoard.Application.Interfaces;
using FlowBoard.Domain.Entities;
using FlowBoard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Project>> GetAllByTenantAsync(Guid tenantId)
    {
        return await _context.Projects
            .Where(p => p.TenantId == tenantId)
            .ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(Guid id, Guid tenantId)
    {
        return await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId);
    }

    public async Task AddAsync(Project project)
    {
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Project project)
    {
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
    }
}
