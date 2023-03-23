using GestionProductos.Persistence;
using GestionProductos.Services;
using GestionProductos.Services.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GestionProductos.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = "Administrador,Comercial,Vendedor")]
	public class ReservaController : ControllerBase {

		private readonly ReservaService _reservaService;
		private readonly UserService _userService;

		public ReservaController(ReservaService reservaService, UserService userService) {
			_reservaService = reservaService;
			_userService = userService;
		}


		[Authorize(Roles = "Administrador,Comercial")]
		[HttpGet("GetReservasComercial")]
		public ActionResult GetReservasComercial() {
			var resp = _reservaService.GetAllReservasSolicitadas();
			return StatusCode(resp.StatusCode, resp.Body ?? resp.Message);
		}

		[Authorize(Roles = "Administrador,Comercial")]
		[HttpPut("RechazarReserva/{idReserva}")]
		public ActionResult RechazarReserva(int idReserva) {
			var resp = _reservaService.RejectReserva(idReserva);
			return StatusCode(resp.StatusCode, resp.Body ?? resp.Message);
		}


		[Authorize(Roles = "Administrador,Comercial")]
		[HttpPut("AprobarReserva/{idReserva}")]
		public ActionResult AprobarReserva(int idReserva) {
			var resp = _reservaService.ApproveReserva(idReserva);
			return StatusCode(resp.StatusCode, resp.Body ?? resp.Message);
		}


		[Authorize(Roles = "Administrador,Comercial")]
		[HttpGet("GetVendedores")]
		public ActionResult GetVendedores() {
			var resp = _userService.GetAllVendedores();
			return StatusCode(resp.StatusCode, resp.Body ?? resp.Message);
		}




		[Authorize(Roles = "Administrador,Vendedor")]
		[HttpGet("GetReservasVendedor")]
		public ActionResult GetReservasVendedor() {
			var identity = HttpContext.User.Identity as ClaimsIdentity;
			var resp = _reservaService.GetAllReservasByVendedor(_userService.GetVendedorFromToken(identity!)!);
			return StatusCode(resp.StatusCode, resp.Body ?? resp.Message);
		}

		[Authorize(Roles = "Administrador,Vendedor")]
		[HttpPost("IngresarReserva")]
		public ActionResult<Reserva> IngresarReserva(ReservaDto request) {
			var identity = HttpContext.User.Identity as ClaimsIdentity;
			var resp = _reservaService.CreateReserva(identity!, request);
			return StatusCode(resp.StatusCode, resp.Body ?? resp.Message);
		}


		[Authorize(Roles = "Administrador,Vendedor")]
		[HttpPut("AutoAprobarReserva/{idReserva}")]
		public ActionResult AutoAprobarReserva(int idReserva) {
			var resp = _reservaService.AutoApproveReserva(idReserva);
			return StatusCode(resp.StatusCode, resp.Body ?? resp.Message);
		}

		[Authorize(Roles = "Administrador,Vendedor")]
		[HttpPut("CancelarReserva/{idReserva}")]
		public ActionResult CancelarReserva(int idReserva) {
			var resp = _reservaService.CancelReserva(idReserva);
			return StatusCode(resp.StatusCode, resp.Body ?? resp.Message);
		}

		[Authorize(Roles = "Administrador,Vendedor")]
		[HttpPut("SolicitarAprobacionReserva/{idReserva}")]
		public ActionResult SolicitarAprobacionReserva(int idReserva) {
			var resp = _reservaService.RequestReserva(idReserva);
			return StatusCode(resp.StatusCode, resp.Body ?? resp.Message);
		}

	}
}
