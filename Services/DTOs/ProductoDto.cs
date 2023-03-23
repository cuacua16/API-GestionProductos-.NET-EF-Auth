using GestionProductos.Persistence;

namespace GestionProductos.Services.DTOs {
	public class ProductoAddDto : ProductoDto {
		public string Codigo { get; set; } = null!;
		public decimal Precio { get; set; }
	}

	public class ProductoEditDto : ProductoDto {
		public int IdProducto { get; set; }
	}

	public class ProductoDto {
		public string? Barrio { get; set; }
		public string? Imagen { get; set; }
	}
}
