using System;
using System.Collections.Generic;

namespace GestionProductos.Persistence {
	public partial class Reserva {
		public Reserva() {
			IdProductos = new HashSet<Producto>();
		}

		public int IdReserva { get; set; }
		public string Cliente { get; set; } = null!;
		public Guid IdVendedor { get; set; }
		public int Estado { get; set; }

		public virtual User IdVendedorNavigation { get; set; } = null!;

		public virtual ICollection<Producto> IdProductos { get; set; }
	}


	public enum EstadoReserva {
		Ingresada = 1,
		Solicitada,
		Cancelada,
		Aprobada,
		Rechazada,

	}
}
