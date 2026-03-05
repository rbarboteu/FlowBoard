using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Application.Interfaces;

public interface ITaskRepository
{
    Task<List<TaskItem>> GetAllByProjectAsync(Guid projectId, Guid tenantId);
    Task<TaskItem?> GetByIdAsync(Guid id, Guid tenantId);
    Task AddAsync(TaskItem task);
    Task UpdateAsync(TaskItem task);
    Task DeleteAsync(TaskItem task);
}
