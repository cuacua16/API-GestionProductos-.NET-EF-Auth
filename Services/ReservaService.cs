using GestionProductos.Persistence;
using GestionProductos.Persistence.Repositories;
using GestionProductos.Services.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestionProductos.Services {
	public class ReservaService {


		private readonly ReservaRepository _reservaRepository;
		private readonly UserService _userService;
		private readonly ProductoService _productoService;

		public ReservaService(ReservaRepository reservaRepository, UserService userService, ProductoService productoService) {
			_reservaRepository = reservaRepository;
			_userService = userService;
			_productoService = productoService;
		}


		public Respuesta GetAllReservas() {
			var reservas = _reservaRepository.FindAll();
			if(reservas == null) return new Respuesta { StatusCode = 404, Message = "No se encontraron reservas" };
			else return new Respuesta { StatusCode = 200, Body = reservas };
		}

		public Respuesta GetAllReservasSolicitadas() {
			var reservas = _reservaRepository.FindAllSolicitadas();
			if(reservas == null) return new Respuesta { StatusCode = 404, Message = "No se encontraron reservas" };
			else return new Respuesta { StatusCode = 200, Body = reservas };
		}


		public Respuesta GetAllReservasByVendedor(UserVendedorDto vendedor) {
			if(vendedor == null) return new Respuesta { StatusCode = 404, Message = "Vendedor no encontrado" };
			var reservas = _reservaRepository.FindAllByIdVendedor(vendedor.Id);
			if(reservas == null) return new Respuesta { StatusCode = 404, Message = "No se encontraron reservas" };
			else return new Respuesta { StatusCode = 200, Body = reservas };
		}


		public int GetCountByVendedor(UserVendedorDto vendedor) {
			return _reservaRepository.GetCountByIdVendedor(vendedor.Id);
		}


		public bool CanAddReserva(UserVendedorDto vendedor) {
			int cantidadReservas = GetCountByVendedor(vendedor);
			if(cantidadReservas >= 3) return false;
			return true;
		}


		public Respuesta CreateReserva(ClaimsIdentity identity, ReservaDto request) {
			var vendedor = _userService.GetVendedorFromToken(identity);
			if(vendedor == null) return new Respuesta { StatusCode = 404, Message = "Vendedor no encontrado" };
			if(!CanAddReserva(vendedor)) return new Respuesta { StatusCode = 403, Message = "El vendedor no puede ingresar más reservas" };

			var producto = _productoService.GetProductoById(request.IdProducto).Body;
			if(producto == null) return new Respuesta { StatusCode = 404, Message = "Producto no encontrado" };
			else if(producto.Estado != (int)EstadoProducto.Disponible) return new Respuesta { StatusCode = 403, Message = "El producto no está disponible para reservar" };

			var reserva = new Reserva() {
				Cliente = request.Cliente,
				IdVendedor = vendedor.Id,
				Estado = (int)EstadoReserva.Ingresada,
			};
			reserva.IdProductos.Add(producto);

			_reservaRepository.AddOne(reserva);
			_productoService.ProductoReservado(producto, reserva);
			return new Respuesta { StatusCode = 201, Body = reserva };
		}


		public void ReservaIngresada(Reserva reserva) {
			_reservaRepository.UpdateEstado(reserva, (int)EstadoReserva.Ingresada);
		}
		public void ReservaSolicitada(Reserva reserva) {
			_reservaRepository.UpdateEstado(reserva, (int)EstadoReserva.Solicitada);
		}
		public void ReservaCancelada(Reserva reserva) {
			_reservaRepository.UpdateEstado(reserva, (int)EstadoReserva.Cancelada);
		}
		public void ReservaAprobada(Reserva reserva) {
			_reservaRepository.UpdateEstado(reserva, (int)EstadoReserva.Aprobada);
		}
		public void ReservaRechazada(Reserva reserva) {
			_reservaRepository.UpdateEstado(reserva, (int)EstadoReserva.Rechazada);
		}




		public Respuesta RejectReserva(int idReserva) {
			var reserva = _reservaRepository.FindOneById(idReserva);
			if(reserva is null) return new Respuesta { StatusCode = 404, Message = "Reserva no encontrada" };
			if(reserva.Estado != (int)EstadoReserva.Solicitada) return new Respuesta { StatusCode = 403, Message = SetMessage(reserva) };

			var producto = _productoService.GetProductoById(reserva.IdProductos.FirstOrDefault()!.IdProducto).Body;
			if(producto is null) return new Respuesta { StatusCode = 404, Message = "Producto no encontrado" };

			_productoService.ProductoDisponible(producto);
			ReservaRechazada(reserva);
			return new Respuesta { StatusCode = 200, Message = "Reserva rechazada satisfactoriamente" };
		}




		public Respuesta ApproveReserva(int idReserva) {
			var reserva = _reservaRepository.FindOneById(idReserva);
			if(reserva is null) return new Respuesta { StatusCode = 404, Message = "Reserva no encontrada" };
			if(reserva.Estado != (int)EstadoReserva.Solicitada) return new Respuesta { StatusCode = 403, Message = SetMessage(reserva) };

			var producto = _productoService.GetProductoById(reserva.IdProductos.FirstOrDefault()!.IdProducto).Body;
			if(producto is null) return new Respuesta { StatusCode = 404, Message = "Producto no encontrado" };

			_productoService.ProductoVendido(producto);
			ReservaAprobada(reserva);
			return new Respuesta { StatusCode = 200, Message = "Reserva aprobada satisfactoriamente" };
		}


		public Respuesta AutoApproveReserva(int idReserva) {
			var reserva = _reservaRepository.FindOneById(idReserva);
			if(reserva is null) return new Respuesta { StatusCode = 404, Message = "Reserva no encontrada" };
			if(reserva.Estado != (int)EstadoReserva.Ingresada) return new Respuesta { StatusCode = 403, Message = SetMessage(reserva) };

			var producto = _productoService.GetProductoById(reserva.IdProductos.FirstOrDefault()!.IdProducto).Body;
			if(producto is null) return new Respuesta { StatusCode = 404, Message = "Producto no encontrado" };

			if(producto.Barrio == null) return new Respuesta { StatusCode = 404, Message = "Barrio no identificado" };
			int productosMismoBarrio = _productoService.GetCountDisponiblesByBarrio(producto.Barrio);
			if(productosMismoBarrio > 1 && producto.Precio >= 100000) return new Respuesta { StatusCode = 403, Message = "El producto debe valer menos de $100.000 o ser el último disponible de su barrio para su autoaprobación" };

			_productoService.ProductoVendido(producto);
			ReservaAprobada(reserva);

			return new Respuesta { StatusCode = 200, Message = "Reserva autoaprobada satisfactoriamente" };
		}


		public Respuesta CancelReserva(int idReserva) {
			var reserva = _reservaRepository.FindOneById(idReserva);
			if(reserva is null) return new Respuesta { StatusCode = 404, Message = "Reserva no encontrada" };
			if(reserva.Estado == (int)EstadoReserva.Cancelada || reserva.Estado == (int)EstadoReserva.Aprobada || reserva.Estado == (int)EstadoReserva.Rechazada) return new Respuesta { StatusCode = 403, Message = SetMessage(reserva) };

			var producto = _productoService.GetProductoById(reserva.IdProductos.FirstOrDefault()!.IdProducto).Body;
			if(producto is null) return new Respuesta { StatusCode = 404, Message = "Producto no encontrado" };

			_productoService.ProductoDisponible(producto);
			ReservaCancelada(reserva);
			return new Respuesta { StatusCode = 200, Message = "Reserva cancelada satisfactoriamente" };
		}



		public Respuesta RequestReserva(int idReserva) {
			var reserva = _reservaRepository.FindOneById(idReserva);
			if(reserva is null) return new Respuesta { StatusCode = 404, Message = "Reserva no encontrada" };
			if(reserva.Estado != (int)EstadoReserva.Ingresada) return new Respuesta { StatusCode = 403, Message = SetMessage(reserva) };

			var producto = _productoService.GetProductoById(reserva.IdProductos.FirstOrDefault()!.IdProducto).Body;
			if(producto is null) return new Respuesta { StatusCode = 404, Message = "Producto no encontrado" };

			ReservaSolicitada(reserva);
			return new Respuesta { StatusCode = 200, Message = "Aprobacion de reserva solicitada satisfactoriamente" };
		}














		private static string GetUserFromToken(ClaimsIdentity identity) {
			if(identity.Claims.Count() == 0) return "Error al identificar usuario";
			return identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)!.Value;
		}


		private string SetMessage(Reserva reserva) {
			switch(reserva.Estado) {
				case (int)EstadoReserva.Ingresada:
				return "La reserva con estado 'Ingresada' no es válida para la operación";
				case (int)EstadoReserva.Solicitada:
				return "La reserva se encuentra solicitada, no es válida para la operación";
				case (int)EstadoReserva.Cancelada:
				return "La reserva ya fue cancelada anteriormente";
				case (int)EstadoReserva.Aprobada:
				return "La reserva ya fue aprobada anteriormente";
				case (int)EstadoReserva.Rechazada:
				return "La reserva fue rechazada, no es válida para la operación";
				default:
				return "Estado de la reserva desconocido";
			}
		}

	}
}
