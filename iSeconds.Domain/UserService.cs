
using System;

namespace iSeconds.Domain
{
	/*
	 * Gerencia o usuario atual, permitindo que as activity's peguem o mesmo.
	 * Tambem implementa a logica de login (se for ter...)
	 */
	public class UserService
	{
		public event EventHandler<GenericEventArgs<User>> OnCurrentUserChanged;

		// TODO: implementar logica de login (se for ter...)
		private User currentUser = null;
		public User CurrentUser {
			get {
				return currentUser;
			}
			set {
				currentUser = value;
				if (OnCurrentUserChanged != null)
					OnCurrentUserChanged(this, new GenericEventArgs<User>(currentUser));
			}
		}
	}
}

