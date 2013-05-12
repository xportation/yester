
using System;

namespace iSeconds.Domain
{
	/*
	 * Gerencia o usuario atual, permitindo que as activity's peguem o mesmo.
	 * Tambem implementa a logica de login (se for ter...)
	 */
	public class UserService
	{
		private IRepository repository;

		public UserService(IRepository repository)
		{
			this.repository= repository;
		}

//		public event EventHandler<GenericEventArgs<User>> OnCurrentUserChanged;

		// TODO: implementar logica de login (se for ter...)
		private User currentUser = null;
		public User CurrentUser {
			get {
				return currentUser;
			}
//			set {
//				currentUser = value;
//				if (OnCurrentUserChanged != null)
//					OnCurrentUserChanged (this, new GenericEventArgs<User> (currentUser));
//			}
		}

		public bool Login(string userName, string password)
		{
			currentUser = repository.GetUser(userName);
			return currentUser != null;
		}

		public void CreateUser(string name)
		{
			User user = new User (name, repository);
			repository.SaveUser (user);
			user.CreateTimeline ("Default Timeline", "This is a default timeline");
			currentUser = user;
		}
	}
}

