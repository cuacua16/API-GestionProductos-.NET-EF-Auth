using System;
using System.Collections.Generic;

namespace GestionProductos.Persistence {
	public partial class Producto {
		public Producto() {
			IdReservas = new HashSet<Reserva>();
		}

		public int IdProducto { get; set; }
		public string Codigo { get; set; } = null!;
		public string? Barrio { get; set; }
		public decimal Precio { get; set; }
		public string? Imagen { get; set; }
		public int Estado { get; set; }

		public virtual ICollection<Reserva> IdReservas { get; set; }
	}


	public enum EstadoProducto {
		Disponible = 1,
		Reservado,
		Vendido,
	}
}
