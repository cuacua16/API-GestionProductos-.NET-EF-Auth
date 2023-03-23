namespace GestionProductos.Services {
	public class Respuesta {
		public int StatusCode { get; set; }
		public dynamic? Body { get; set; }
		public string? Message { get; set; } = string.Empty;
	}
}
