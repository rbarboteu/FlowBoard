using FlowBoard.Application.DTOs.Auth;
using FlowBoard.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (result is null)
            return Unauthorized(new { error = "Credenciais inválidas." });

        return Ok(result);
    }

    [HttpPost("register-tenant")]
    public async Task<IActionResult> RegisterTenant([FromBody] RegisterTenantRequest request)
    {
        var result = await _authService.RegisterTenantAsync(request);
        if (result is null)
            return BadRequest(new { error = "Slug já em uso." });
        return Ok(result);
    }

    [HttpPost("register-user")]
    [Authorize]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
    {
        var tenantIdClaim = User.Claims.FirstOrDefault(c => c.Type == "tenantId");

        var roleClaim = User.Claims.FirstOrDefault(c => c.Type == "role")
                     ?? User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role);

        if (tenantIdClaim is null)
            return Unauthorized(new { error = "Token inválido." });

        var roleValue = roleClaim?.Value ?? "Member";

        if (roleValue != "Admin")
            return Forbid();

        var tenantId = Guid.Parse(tenantIdClaim.Value);
        var result = await _authService.RegisterUserAsync(request, tenantId);

        if (result is null)
            return BadRequest(new { error = "Email já cadastrado." });

        return Ok(result);
    }
}