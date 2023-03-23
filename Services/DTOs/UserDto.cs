using GestionProductos.Persistence;

namespace GestionProductos.Services.DTOs;

public class UserVendedorDto {
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Username { get; set; }
	public virtual ICollection<Role> Roles { get; set; }
	public virtual ICollection<Reserva> Reservas { get; set; }
}

public class UserRegisterDto : UserDto {
	public string Role { get; set; } = string.Empty;
}


public class UserDto {
	public string Username { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
}
