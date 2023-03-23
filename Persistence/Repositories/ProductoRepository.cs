using GestionProductos.Persistence;
using GestionProductos.Services.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GestionProductos.Persistence.Repositories {
	public class ProductoRepository {

		private readonly AplicacionDbContext _context;

		public ProductoRepository(AplicacionDbContext context) {
			_context = context;
		}

		public IEnumerable<Producto> FindAll() {
			return _context.Productos.Include(x => x.IdReservas).ToList();
		}

		public Producto? FindOneById(int id) {
			return _context.Productos.Include(x => x.IdReservas).FirstOrDefault(x => x.IdProducto == id);
		}

		public int CountDisponiblesByBarrio(string barrio) {
			return _context.Productos.Where(x => x.Barrio == barrio && x.Estado == (int)EstadoProducto.Disponible).ToList().Count();
		}


		public Producto? AddOne(Producto producto) {
			_context.Productos.Add(producto);
			_context.SaveChanges();
			return producto;
		}

		public Producto? UpdateOne(ProductoEditDto newProducto) {
			var producto = FindOneById(newProducto.IdProducto);
			if(producto is not null) {
				producto.Barrio = newProducto.Barrio;
				producto.Imagen = newProducto.Imagen;
			}
			_context.SaveChanges();
			return producto;
		}

		public void RemoveOne(Producto producto) {
			producto.IdReservas.Clear();
			_context.Productos.Remove(producto);
			_context.SaveChanges();
		}


		public void UpdateEstado(Producto producto, int estado, Reserva? reserva = null) {
			if(reserva != null) producto.IdReservas.Add(reserva);
			producto.Estado = estado;
			_context.SaveChanges();
		}

	}
}
