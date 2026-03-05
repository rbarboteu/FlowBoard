using FlowBoard.Application.Interfaces;
using FlowBoard.Domain.Entities;
using FlowBoard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAndTenantAsync(string email, Guid tenantId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.TenantId == tenantId);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
    public async Task<Tenant?> GetTenantBySlugAsync(string slug)
    {
        return await _context.Tenants
            .FirstOrDefaultAsync(t => t.Slug == slug.ToLower());
    }
    public async Task<bool> EmailExistsAsync(string email, Guid tenantId)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email && u.TenantId == tenantId);
    }

    public async Task AddTenantAsync(Tenant tenant)
    {
        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();
    }
    public async Task<List<User>> GetAllByTenantAsync(Guid tenantId)
    {
        return await _context.Users
            .Where(u => u.TenantId == tenantId)
            .ToListAsync();
    }
}