using GestionProductos.Persistence;
using GestionProductos.Persistence.Repositories;
using GestionProductos.Services.DTOs;

namespace GestionProductos.Services {
	public class ProductoService {


		private readonly ProductoRepository _productoRepository;

		public ProductoService(ProductoRepository productoRepository) {
			_productoRepository = productoRepository;
		}


		public Respuesta GetAllProductos() {
			var productos = _productoRepository.FindAll();
			if(productos == null) return new Respuesta { StatusCode = 404, Message = "No se encontraron productos" };
			else return new Respuesta { StatusCode = 200, Body = productos };
		}


		public int GetCountDisponiblesByBarrio(string barrio) {
			return _productoRepository.CountDisponiblesByBarrio(barrio);
		}

		public Respuesta GetProductoById(int id) {
			var producto = _productoRepository.FindOneById(id);
			if(producto is null) return new Respuesta { StatusCode = 404, Message = "Producto no encontrado" };
			else return new Respuesta { StatusCode = 200, Body = producto };
		}


		public Respuesta CreateProducto(ProductoAddDto request) {
			if(request is null) return new Respuesta { StatusCode = 400, Message = "Error al crear el producto" };
			var producto = new Producto() {
				Codigo = request.Codigo,
				Barrio = request.Barrio,
				Precio = request.Precio,
				Imagen = request.Imagen,
				Estado = (int)EstadoProducto.Disponible
			};
			_productoRepository.AddOne(producto);
			return new Respuesta { StatusCode = 200, Body = producto };
		}

		public Respuesta EditProducto(ProductoEditDto request) {
			var producto = _productoRepository.FindOneById(request.IdProducto);
			if(producto == null) return new Respuesta { StatusCode = 404, Message = "Producto no encontrado" };
			if(producto.Estado == (int)EstadoProducto.Vendido) return new Respuesta { StatusCode = 403, Message = "El producto ya fue vendido anteriormente" };
			_productoRepository.UpdateOne(request);
			return new Respuesta { StatusCode = 200, Body = producto };
		}


		public Respuesta DeleteProducto(int idProducto) {
			var producto = _productoRepository.FindOneById(idProducto);
			if(producto == null) return new Respuesta { StatusCode = 404, Message = "Producto no encontrado" };
			if(producto.Estado == (int)EstadoProducto.Reservado) return new Respuesta { StatusCode = 403, Message = "El producto ya fue reservado anteriormente" };
			if(producto.Estado == (int)EstadoProducto.Vendido) return new Respuesta { StatusCode = 403, Message = "El producto ya fue vendido anteriormente" };
			_productoRepository.RemoveOne(producto);
			return new Respuesta { StatusCode = 200, Message = "Producto con id: " + idProducto + " eliminado satisfactoriamente" };
		}




		public void ProductoDisponible(Producto producto) {
			_productoRepository.UpdateEstado(producto, (int)EstadoProducto.Disponible);
		}
		public void ProductoReservado(Producto producto, Reserva reserva) {
			_productoRepository.UpdateEstado(producto, (int)EstadoProducto.Reservado, reserva);
		}
		public void ProductoVendido(Producto producto) {
			_productoRepository.UpdateEstado(producto, (int)EstadoProducto.Vendido);
		}


	}
}
