
using System;

namespace iSeconds.Domain
{
	/*
	 * Gerencia o usuario atual, permitindo que as activity's peguem o mesmo.
	 * Tambem implementa a logica de login (se for ter...)
	 */
	public class UserService
	{
		private I18nService i18n;
		private IRepository repository;

		public UserService(IRepository repository, I18nService i18n)
		{
			this.i18n = i18n;
			this.repository= repository;
		}

		private User currentUser = null;
		public User CurrentUser {
			get {
				return currentUser;
			}
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
			user.CreateTimeline (i18n.Msg("Default Timeline"), i18n.Msg("This is a default timeline"));
			currentUser = user;
		}
	}
}

