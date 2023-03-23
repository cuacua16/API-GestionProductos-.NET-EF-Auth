using GestionProductos.Persistence;
using GestionProductos.Services.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestionProductos.Persistence.Repositories {
	public class UserRepository {

		private readonly AplicacionDbContext _context;

		public UserRepository(AplicacionDbContext context) {
			_context = context;
		}

		public IEnumerable<User> FindAll() {
			return _context.Users.Include(x => x.Roles).Include(x => x.Reservas).ToList();
		}


		public IEnumerable<User> FindAllVendedores() {
			return _context.Users.Where(x => x.Roles.First().Name == "Vendedor" || x.Roles.First().Name == "Administrador").Include(x => x.Reservas).ThenInclude(x => x.IdProductos).ToList();
		}

		public User? FindOneByName(string name) {
			return _context.Users.Include(x => x.Roles).Include(x => x.Reservas).FirstOrDefault(x => x.Name == name);
		}


		public User? FindOneById(Guid id) {
			return _context.Users.Include(x => x.Roles).Include(x => x.Reservas).FirstOrDefault(x => x.Id == id);
		}


		public User? Add(User user) {
			_context.Users.Add(user);
			_context.SaveChanges();
			return _context.Users.Include(x => x.Roles).FirstOrDefault(x => x.Id == user.Id);
		}

		public void AddRoleToUser(Guid idUser, Role role) {
			var user = FindOneById(idUser);
			user?.Roles.Add(role);
			_context.SaveChanges();
		}






	}
}
