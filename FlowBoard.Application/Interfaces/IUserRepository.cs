using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAndTenantAsync(string email, Guid tenantId);
    Task<User?> GetByIdAsync(Guid id);
    Task AddAsync(User user);
    Task<Tenant?> GetTenantBySlugAsync(string slug);
    Task<bool> EmailExistsAsync(string email, Guid tenantId);
    Task AddTenantAsync(Tenant tenant);
    Task<List<User>> GetAllByTenantAsync(Guid tenantId);
}
