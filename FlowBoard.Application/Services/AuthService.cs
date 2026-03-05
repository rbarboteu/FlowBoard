using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowBoard.Application.DTOs.Auth;
using FlowBoard.Application.DTOs.Projects;
using FlowBoard.Application.Interfaces;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Enums;

namespace FlowBoard.Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthService(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        // Resolve o tenant pelo slug
        var tenant = await _userRepository.GetTenantBySlugAsync(request.Slug);
        if (tenant is null) return null;

        var user = await _userRepository.GetByEmailAndTenantAsync(request.Email, tenant.Id);
        if (user is null) return null;

        bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isValid) return null;

        var token = _jwtService.GenerateToken(user);

        return new LoginResponse
        {
            Token = token,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }
    public async Task<LoginResponse?> RegisterTenantAsync(RegisterTenantRequest request)
    {
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = request.CompanyName,
            Slug = request.Slug.ToLower().Trim()
        };

        await _userRepository.AddTenantAsync(tenant);

        var admin = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            Email = request.AdminEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.AdminPassword),
            Role = UserRole.Admin
        };

        await _userRepository.AddAsync(admin);

        var token = _jwtService.GenerateToken(admin);

        return new LoginResponse
        {
            Token = token,
            Email = admin.Email,
            Role = admin.Role.ToString()
        };
    }

    public async Task<LoginResponse?> RegisterUserAsync(RegisterUserRequest request, Guid tenantId)
    {
        var exists = await _userRepository.EmailExistsAsync(request.Email, tenantId);
        if (exists) return null;

        var role = Enum.TryParse<UserRole>(request.Role, out var parsedRole)
            ? parsedRole : UserRole.Member;

        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = role
        };

        await _userRepository.AddAsync(user);

        var token = _jwtService.GenerateToken(user);

        return new LoginResponse
        {
            Token = token,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }
}