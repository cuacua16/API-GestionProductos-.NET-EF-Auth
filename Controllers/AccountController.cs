using GestionProductos.Persistence;
using GestionProductos.Services;
using GestionProductos.Services.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace GestionProductos.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase {

	private readonly UserService _userService;
	private readonly RoleService _roleService;

	public AccountController(UserService userService, RoleService roleService) {
		_userService = userService;
		_roleService = roleService;
	}



	[HttpPost("Register")]
	[AllowAnonymous]
	public ActionResult<User> Register(UserRegisterDto request) {
		var resp = _userService.CreateUser(request);
		return StatusCode(resp.StatusCode, resp.Body ?? resp.Message);
	}


	[HttpPost("Login")]
	[AllowAnonymous]
	public ActionResult<string> Login(UserDto request) {
		var resp = _userService.Login(request);
		return StatusCode(resp.StatusCode, resp.Body ?? resp.Message);
	}


	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
	[HttpPost("CreateRole")]
	public ActionResult<string> Role(RoleDto request) {
		var resp = _roleService.CreateRole(request.Name);
		return StatusCode(resp.StatusCode, resp.Body ?? resp.Message);

	}


	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
	[HttpPut("/api/Account/User/{userId:Guid}/AddRole/{roleId:Guid}")]
	public ActionResult<string> AddRole(Guid userId, Guid roleId) {
		var resp = _userService.AddRoleToUser(userId, roleId);
		return StatusCode(resp.StatusCode, resp.Body ?? resp.Message);
	}


	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
	[HttpGet("GetUsers")]
	public ActionResult Users() {
		var resp = _userService.GetAllUsers();
		return StatusCode(resp.StatusCode, resp.Body ?? resp.Message);
	}

	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
	[HttpGet("GetRoles")]
	public ActionResult Roles() {
		var resp = _roleService.GetAllRoles();
		return StatusCode(resp.StatusCode, resp.Body ?? resp.Message);
	}


}
