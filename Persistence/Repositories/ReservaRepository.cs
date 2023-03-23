using GestionProductos.Services;
using Microsoft.EntityFrameworkCore;

namespace GestionProductos.Persistence.Repositories {
	public class ReservaRepository {


		private readonly AplicacionDbContext _context;

		public ReservaRepository(AplicacionDbContext context) {
			_context = context;
		}

		public IEnumerable<Reserva> FindAll() {
			return _context.Reservas.Include(x => x.IdProductos).Include(x => x.IdVendedorNavigation).ToList();
		}


		public IEnumerable<Reserva> FindAllSolicitadas() {
			return _context.Reservas.Where(x => x.Estado == (int)EstadoReserva.Solicitada).Include(x => x.IdProductos).Include(x => x.IdVendedorNavigation).ToList();
		}


		public IEnumerable<Reserva> FindAllByIdVendedor(Guid id) {
			return _context.Reservas.Where(x => x.IdVendedor == id && (x.Estado == (int)EstadoReserva.Ingresada || x.Estado == (int)EstadoReserva.Solicitada)).Include(x => x.IdProductos).ToList();
		}

		public Reserva? FindOneById(int idReserva) {
			var reserva = _context.Reservas.Include(x => x.IdProductos).FirstOrDefault(x => x.IdReserva == idReserva);
			return reserva;
		}

		public int GetCountByIdVendedor(Guid id) {
			return _context.Reservas.Where(x => x.IdVendedor == id && (x.Estado == (int)EstadoReserva.Ingresada || x.Estado == (int)EstadoReserva.Solicitada)).Include(x => x.IdProductos).ToList().Count();
		}



		public Reserva? AddOne(Reserva reserva) {
			_context.Reservas.Add(reserva);
			_context.SaveChanges();
			return reserva;
		}


		public void UpdateEstado(Reserva reserva, int estado) {
			reserva.Estado = estado;
			_context.SaveChanges();
		}





	}
}
