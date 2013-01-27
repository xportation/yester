
using System;

namespace iSeconds.Domain
{
	/*
	 * Gerencia o usuario atual, permitindo que as activity's peguem o mesmo.
	 * Tambem implementa a logica de login (se for ter...)
	 */
	public class UserService
	{
		public event EventHandler<GenericEventArgs<User>> OnActualUserChanged;

		// TODO: implementar logica de login (se for ter...)
		private User actualUser = new User();
		public User ActualUser {
			get {
				return actualUser;
			}
			set {
				actualUser = value;
				if (OnActualUserChanged != null)
					OnActualUserChanged(this, new GenericEventArgs<User>(actualUser));
			}
		}
	}
}

