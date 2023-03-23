using GestionProductos.Persistence;
using GestionProductos.Services.DTOs;

namespace GestionProductos.Persistence.Repositories {
	public class RoleRepository {

		private readonly AplicacionDbContext _context;

		public RoleRepository(AplicacionDbContext context) {
			_context = context;
		}

		public IEnumerable<Role> FindAll() {
			return _context.Roles.ToList();
		}

		public Role? FindOneByName(string name) {
			return _context.Roles.FirstOrDefault(x => x.Name == name);
		}

		public Role? FindOneById(Guid id) {
			return _context.Roles.FirstOrDefault(x => x.Id == id);
		}


		public Role? Add(Role role) {
			if(role is null) return role;
			_context.Roles.Add(role);
			_context.SaveChanges();
			return role;
		}















	}
}
