
using System;

namespace iSeconds.Domain
{
	/*
	 * Gerencia o usuario atual, permitindo que as activity's peguem o mesmo.
	 * Tambem implementa a logica de login (se for ter...)
	 */
	public class UserService
	{
		// TODO: implementar logica de login (se for ter...)
		private User actualUser = new User();

		public User GetActualUser ()
		{
			return actualUser;
		}

	}
}

