using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Application.Interfaces;

public interface IProjectRepository
{
    Task<List<Project>> GetAllByTenantAsync(Guid tenantId);
    Task<Project?> GetByIdAsync(Guid id, Guid tenantId);
    Task AddAsync(Project project);
    Task UpdateAsync(Project project);
    Task DeleteAsync(Project project);
}