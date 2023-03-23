using GestionProductos.Persistence;
using GestionProductos.Services;
using GestionProductos.Services.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionProductos.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = "Administrador,Vendedor")]
	public class ProductoController : ControllerBase {

		private readonly ProductoService _productoService;

		public ProductoController(ProductoService productoService) {
			_productoService = productoService;
		}



		[HttpGet("GetProductos")]
		public ActionResult GetProductos() {
			var resp = _productoService.GetAllProductos();
			return StatusCode(resp.StatusCode, resp.Body ?? resp.Message);
		}



		[HttpGet("GetProducto/{idProducto}")]
		public ActionResult GetProducto(int idProducto) {
			var resp = _productoService.GetProductoById(idProducto);
			return StatusCode(resp.StatusCode, resp.Body ?? resp.Message);
		}



		[HttpPost("AddProducto")]
		public ActionResult<Producto> AddProducto(ProductoAddDto request) {
			var resp = _productoService.CreateProducto(request);
			return StatusCode(resp.StatusCode, resp.Body ?? resp.Message);
		}



		[HttpPut("EditProducto")]
		public ActionResult<Producto> EditProducto(ProductoEditDto request) {
			var resp = _productoService.EditProducto(request);
			return StatusCode(resp.StatusCode, resp.Body ?? resp.Message);
		}



		[HttpDelete("DeleteProducto/{idProducto}")]
		public ActionResult DeleteProducto(int idProducto) {
			var resp = _productoService.DeleteProducto(idProducto);
			return StatusCode(resp.StatusCode, resp.Body ?? resp.Message);
		}

	}
}
