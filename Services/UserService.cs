using GestionProductos.Persistence;
using GestionProductos.Persistence.Repositories;
using GestionProductos.Services.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace GestionProductos.Services {

	public class UserService {

		private readonly UserRepository _userRepository;
		private readonly RoleService _roleService;
		private readonly IConfiguration _configuration;

		public UserService(UserRepository userRepository, RoleService roleService, IConfiguration configuration) {
			_userRepository = userRepository;
			_roleService = roleService;
			_configuration = configuration;
		}




		public Respuesta GetAllUsers() {
			var users = _userRepository.FindAll();
			if(users == null) return new Respuesta { StatusCode = 404, Message = "Usuarios no encontrados" };
			else return new Respuesta { StatusCode = 200, Body = users };

		}




		public Respuesta GetAllVendedores() {
			var vendedores = _userRepository.FindAllVendedores();
			if(vendedores == null) return new Respuesta { StatusCode = 404, Message = "Usuarios no encontrados" };
			else return new Respuesta { StatusCode = 200, Body = vendedores.Select(x => new { x.Id, reservas = x.Reservas.Select(y => new { y.Estado, y.Cliente, y.IdReserva, y.IdVendedor, idProductos = y.IdProductos.Select(z => new { z.IdProducto, z.Estado, z.Precio, z.Barrio, z.Imagen, z.Codigo }) }), x.Username, x.Name, x.Roles }) };
		}




		public Respuesta GetUserByName(string name) {
			var user = _userRepository.FindOneByName(name);
			if(user == null) return new Respuesta { StatusCode = 404, Message = "Usuario no encontrado" };
			else return new Respuesta { StatusCode = 200, Body = user };
		}




		public Respuesta GetVendedorByName(string name) {
			var user = _userRepository.FindOneByName(name);
			if(user == null) return new Respuesta { StatusCode = 404, Message = "Usuario no encontrado" };
			if(user.Roles?.FirstOrDefault()?.Name == "Vendedor" || user.Roles?.FirstOrDefault()?.Name == "Administrador") {
				var vendedor = new UserVendedorDto {
					Id = user.Id,
					Name = user.Name,
					Username = user.Username,
					Roles = user.Roles,
					Reservas = user.Reservas
				};
				return new Respuesta { StatusCode = 200, Body = vendedor };
			} else return new Respuesta { StatusCode = 400, Message = "Vendedor no encontrado" };
		}




		public Respuesta GetUserById(Guid id) {
			var user = _userRepository.FindOneById(id);
			if(user != null) return new Respuesta { StatusCode = 200, Body = user };
			else return new Respuesta { StatusCode = 400, Message = "Usuario no encontrado" };
		}



		public Respuesta GetVendedorById(Guid id) {
			var user = _userRepository.FindOneById(id);
			if(user is null) return new Respuesta { StatusCode = 404, Message = "Usuario no encontrado" };
			if(user.Roles?.FirstOrDefault()?.Name == "Vendedor" || user.Roles?.FirstOrDefault()?.Name == "Administrador") {
				var vendedor = new UserVendedorDto {
					Id = user.Id,
					Name = user.Name,
					Username = user.Username,
					Roles = user.Roles,
					Reservas = user.Reservas
				};
				return new Respuesta { StatusCode = 200, Body = vendedor };
			} else return new Respuesta { StatusCode = 400, Message = "Vendedor no encontrado" };
		}



		public Respuesta CreateUser(UserRegisterDto request) {
			CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
			var exists = _userRepository.FindOneByName(request.Username);
			if(exists != null) return new Respuesta { StatusCode = 403, Message = "El nombre de usuario ya existe" };
			var user = new User() {
				Id = Guid.NewGuid(),
				Username = request.Username,
				Name = request.Username,
				PasswordHash = passwordHash,
				PasswordSalt = passwordSalt,
				Roles = new List<Role>(),
				Reservas = new List<Reserva>()
			};
			var role = _roleService.GetRoleByName(request.Role).Body;
			if(role is null) {
				return new Respuesta { StatusCode = 400, Message = "Rol inexistente" };
			}
			user.Roles.Add(role);
			if(_userRepository.Add(user) == user) return new Respuesta { StatusCode = 201, Body = new { user?.Id, user?.Username, Role = user?.Roles.First().Name } };
			else return new Respuesta { StatusCode = 400, Message = "No se pudo crear el usuario" };

		}



		public Respuesta Login(UserDto request) {
			var user = _userRepository.FindOneByName(request.Username);
			if(user is null) return new Respuesta { StatusCode = 404, Message = "Usuario no encontrado" };
			if(!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt)) {
				return new Respuesta { StatusCode = 403, Message = "Contraseña incorrecta" };
			} else {
				string token = CreateToken(user);
				return new Respuesta { StatusCode = 200, Body = token };
			}

		}


		public Respuesta AddRoleToUser(Guid idUser, Guid idRole) {
			var role = _roleService.GetRoleById(idRole).Body;
			if(role is null) return new Respuesta { StatusCode = 400, Message = "Rol inexistente" };
			if(GetUserById(idUser) is null) return new Respuesta { StatusCode = 400, Message = "Usuario inexistente" };
			else {
				_userRepository.AddRoleToUser(idUser, role);
				return new Respuesta { StatusCode = 200, Message = "Rol agregado exitosamente" };
			}

		}


		public UserVendedorDto? GetVendedorFromToken(ClaimsIdentity identity) {
			string username = GetUserFromToken(identity);
			var vendedor = GetVendedorByName(username).Body;
			return vendedor;
		}











		private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) {
			using(var hmac = new HMACSHA512()) {
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			}
		}

		private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt) {
			using(var hmac = new HMACSHA512(passwordSalt)) {
				var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
				return computedHash.SequenceEqual(passwordHash);
			}
		}

		private string CreateToken(User user) {
			var claims = new List<Claim>
			{
			new Claim(ClaimTypes.Name, user.Username)
		};

			string roles = string.Join(",", user.Roles.Select(x => x.Name));

			claims.Add(new Claim(ClaimTypes.Role, roles));

			var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("Token:Key").Value));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

			var token = new JwtSecurityToken(
				claims: claims,
				expires: DateTime.Now.AddDays(7),
				signingCredentials: creds);

			var jwt = new JwtSecurityTokenHandler().WriteToken(token);

			return jwt;
		}


		private static string GetUserFromToken(ClaimsIdentity identity) {
			if(identity.Claims.Count() == 0) return "Error al identificar usuario";
			return identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
		}


	}
}
