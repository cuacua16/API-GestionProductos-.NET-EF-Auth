﻿using System;
using System.Collections.Generic;

namespace GestionProductos.Persistence {
	public partial class Role {
		public Role() {
			Users = new HashSet<User>();
		}

		public Guid Id { get; set; }
		public string Name { get; set; } = null!;

		public virtual ICollection<User> Users { get; set; }
	}
}
