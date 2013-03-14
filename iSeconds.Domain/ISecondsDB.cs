using System;
using SQLite;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace iSeconds.Domain
{
    public class ISecondsDB : SQLiteConnection, IRepository
    {
		static object locker = new object ();

        public ISecondsDB(string path)
            : base(path)
        {
			Console.WriteLine ("------------------------");
			Console.WriteLine (path);
			Console.WriteLine ("------------------------");

			CreateTable<Timeline>();
            CreateTable<User>();
			CreateTable<DayInfo>();
			CreateTable<MediaInfo>();
        }

        public static string DatabaseFilePath
        {
            get
            {
                // codigo copiado dos samples do xamarin.. serve para guardar o db no lugar correto de acordo com o SO
                //				#if SILVERLIGHT
                //				var path = "MwcDB.db3";
                //				#else

                //				#if __ANDROID__
                string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); ;
                //				#else
                //				// we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
                //				// (they don't want non-user-generated data in Documents)
                //				string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
                //				string libraryPath = Path.Combine (documentsPath, "../Library/");
                //				#endif
                var path = Path.Combine(libraryPath, "ISeconds.db3");
                //				#endif		
                return path;
            }
        }

		public User GetUser (int id)
		{
			//return GetItem<User> (id);
            lock (locker)
            {
                User user = (from i in Table<User>() where i.Id == id select i).First();
                user.LoadTimelines(this.GetUserTimelines(id));
                return user;
            }
		}

        public event EventHandler<GenericEventArgs<Timeline>> OnNewTimeline;

        public void SaveTimeline(Timeline timeline)
        {
            this.SaveItem(timeline);

            if (OnNewTimeline != null)
                OnNewTimeline(this, new GenericEventArgs<Timeline>(timeline));
        }

		public IList<User> GetUsers ()
		{
			return this.GetItems<User> ();
		}

		public IList<Timeline> GetTimelines ()
		{
			return this.GetItems<Timeline> ();
		}

		public IList<DayInfo> GetDays ()
		{
			return this.GetItems<DayInfo>();
		}

		public IList<DayInfo> GetDaysInMonth (int timelineId, int month, int year)
		{
			lock (locker) {
                DateTime begin = new DateTime(year, month, 1);
                DateTime end = new DateTime(year, month, DateTime.DaysInMonth(year, month));

				return (from i in Table<DayInfo>() 
				        	where i.TimelineId == timelineId && i.Date >= begin && i.Date <= end select i
				       ).ToList();
			}
		}

		public IList<Timeline> GetUserTimelines (int userId)
		{
			lock (locker) {
				return (from i in Table<Timeline>() where i.UserId == userId select i).ToList();
			}
		}

		public IList<T> GetItems<T> () where T : IModel, new ()
		{
			lock (locker) {
				return (from i in Table<T> () select i).ToList ();
			}
		}
		
		public T GetItem<T> (int id) where T : IModel, new ()
		{
			lock (locker) {
				return (from i in Table<T> ()
				        where i.Id == id
				        select i).FirstOrDefault ();
			}
		}
		
		public int SaveItem<T> (T item) where T : IModel
		{
			lock (locker) {
				if (item.Id != 0) {
					Update (item);
					return item.Id;
				} else {
					return Insert (item);
				}
			}
		}
		
		public int DeleteItem<T>(int id) where T : IModel, new ()
		{
			lock (locker) {
				return Delete<T> (new T () { Id = id });
			}
		}


    }
}

