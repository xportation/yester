//using System;
//using System.Collections.Generic;
//
//namespace iSeconds.Domain
//{
//	public class RepositoryService : IRepository
//	{
//		private IPersistence persistence = null;
//		UserService userService = null;
//
//		public RepositoryService (UserService userService, IPersistence persistence)
//		{
//			//this.db = new ISecondsDB(ISecondsDB.DatabaseFilePath);
//			this.persistence = persistence;
//			this.userService = userService;
//
//			this.userService.OnCurrentUserChanged += connectToActualUser;
//		}
//
////		public User GetUser (int id)
////		{
////			return persistence.GetUser (id);
////		}
////
////		public IList<User> GetUsers ()
////		{
////			return persistence.GetUsers();
////		}
////
////		public IList<Timeline> GetTimelines ()
////		{
////			return persistence.GetTimelines();
////		}
////
////		public IList<DayInfo> GetDays ()
////		{
////			return persistence.GetDays();
////		}
////
//		void connectToActualUser(object source, GenericEventArgs<User> arg)
//		{
//			User actualUser = arg.Value;
//			persistence.SaveItem (actualUser);
//			actualUser.OnNewTimeline += saveNewTimeline;
//
//			actualUser.LoadTimelines (persistence.GetUserTimelines(actualUser.Id));
//		}
//
//		void saveNewTimeline (object source, GenericEventArgs<Timeline> arg)
//		{
//			Timeline timeline = arg.Value;
//			persistence.SaveItem (timeline);
//			timeline.OnDayChanged += saveDay;
//		}
//
//		void saveDay(object source, GenericEventArgs<DayInfo> arg)
//		{
//			DayInfo day = arg.Value;
//			persistence.SaveItem (day);
//			// TODO: persist day
//
//		}
//	}
//}
//
