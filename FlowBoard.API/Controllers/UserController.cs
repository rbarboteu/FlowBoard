using FlowBoard.Application.DTOs.Users;
using FlowBoard.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    private Guid GetTenantId() =>
        Guid.Parse(User.Claims.First(c => c.Type == "tenantId").Value);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userRepository.GetAllByTenantAsync(GetTenantId());

        var response = users.Select(u => new UserResponse
        {
            Id = u.Id,
            Email = u.Email,
            Role = u.Role.ToString()
        });

        return Ok(response);
    }
}