using GestionProductos.Persistence;

namespace GestionProductos.Services.DTOs {
	public class ReservaDto {
		public string Cliente { get; set; } = string.Empty;
		public int IdProducto { get; set; }
	}
}
