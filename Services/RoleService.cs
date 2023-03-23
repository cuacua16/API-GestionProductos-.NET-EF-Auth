using GestionProductos.Persistence;
using GestionProductos.Persistence.Repositories;
using GestionProductos.Services.DTOs;

namespace GestionProductos.Services {
	public class RoleService {

		private readonly RoleRepository _roleRepository;

		public RoleService(RoleRepository roleRepository) {
			_roleRepository = roleRepository;
		}


		public Respuesta GetAllRoles() {
			var roles = _roleRepository.FindAll();
			if(roles == null) return new Respuesta { StatusCode = 404, Message = "Roles no encontrados" };
			else return new Respuesta { StatusCode = 200, Body = roles };
		}

		public Respuesta GetRoleById(Guid id) {
			var role = _roleRepository.FindOneById(id);
			if(role == null) return new Respuesta { StatusCode = 404, Message = "Rol no encontrado" };
			else return new Respuesta { StatusCode = 200, Body = role };
		}

		public Respuesta GetRoleByName(string name) {
			var role = _roleRepository.FindOneByName(name);
			if(role == null) return new Respuesta { StatusCode = 404, Message = "Rol no encontrado" };
			else return new Respuesta { StatusCode = 200, Body = role };
		}

		public Respuesta CreateRole(string name) {
			var role = _roleRepository.Add(new Role { Name = name, Id = Guid.NewGuid() });
			if(role is null) return new Respuesta { StatusCode = 400, Message = "Error al crear el rol" };
			else return new Respuesta { StatusCode = 200, Body = role };

		}

	}
}
