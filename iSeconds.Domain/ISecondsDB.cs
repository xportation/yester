using System;
using SQLite;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace iSeconds.Domain
{
	public class ISecondsDB : SQLiteConnection, IRepository
	{
		private static object locker = new object ();

		public ISecondsDB (string path)
			: base(path)
		{
			Console.WriteLine ("------------------------");
			Console.WriteLine (path);
			Console.WriteLine ("------------------------");

			//#if DEBUG
			CreateTable<Timeline> ();
			CreateTable<User> ();
			CreateTable<DayInfo> ();
			CreateTable<MediaInfo> ();
			CreateTable<Compilation> ();
			//#endif
		}

		public event EventHandler<GenericEventArgs<Timeline>> OnSaveTimeline;
		public event EventHandler<GenericEventArgs<Timeline>> OnDeleteTimeline;

		public void SaveTimeline (Timeline timeline)
		{
			this.SaveItem (timeline);

			if (OnSaveTimeline != null)
				OnSaveTimeline (this, new GenericEventArgs<Timeline> (timeline));
		}

		public void DeleteTimeline (Timeline timeline)
		{
			this.DeleteItem (timeline);

			if (OnDeleteTimeline != null)
				OnDeleteTimeline (this, new GenericEventArgs<Timeline> (timeline));
		}

		public IList<MediaInfo> GetMediasForDay (DayInfo day)
		{
			return (from i in Table<MediaInfo> () where i.DayId == day.Id select i).ToList ();
		}

		public DayInfo GetDayInfoAt (DateTime dateTime, int timelineId)
		{
			DayInfo dayInfo =
            (from i in Table<DayInfo> () where i.TimelineId == timelineId && i.Date == dateTime select i).FirstOrDefault ();
			if (dayInfo == null) {
				dayInfo = new DayInfo (dateTime, timelineId);
			}
			dayInfo.SetRepository (this);

			return dayInfo;
		}

		public DayInfo GetDayInfo(int dayId)
		{
			return (from i in Table<DayInfo> () where i.Id == dayId select i).FirstOrDefault ();
		}

		public MediaInfo GetMediaById (int id)
		{
			lock (locker) {
				return (from i in Table<MediaInfo> () where i.Id == id select i).FirstOrDefault ();
			}
		}

		public MediaInfo GetMediaByPath (string videopath)
		{
			lock (locker) {
				return (from i in Table<MediaInfo> () where i.Path == videopath select i).FirstOrDefault ();
			}
		}

		public void DeleteMedia (MediaInfo media)
		{
			this.DeleteItem (media);
		}

		public User GetUser (string userName)
		{
			lock (locker) {
				try {
					User user = (from i in Table<User> () where i.Name == userName select i).First ();
					user.SetRepository (this);
					return user;
				} catch (Exception exception) {
					Console.WriteLine (exception.Message);
				}
				return null;
			}
		}

		public void SaveUser (User user)
		{
			this.SaveItem (user);
		}

		public IList<Timeline> GetUserTimelines (int userId)
		{
			lock (locker) {
				IList<Timeline> timelines = (from i in Table<Timeline> () where i.UserId == userId select i).ToList ();
				foreach (Timeline timeline in timelines) {
					timeline.SetRepository (this);
				}
				//return (from i in Table<Timeline>() where i.UserId == userId select i).ToList();
				return timelines;
			}
		}

		public Timeline GetUserTimeline (int userId, int timelineId)
		{
			lock (locker) {
				Timeline timeline =
               (from i in Table<Timeline> () where i.UserId == userId && i.Id == timelineId select i).FirstOrDefault ();
				timeline.SetRepository (this);
				return timeline;
			}
		}

		public Timeline GetTimeline (int id)
		{
			lock (locker) {
				Timeline timeline =
               (from i in Table<Timeline> () where i.Id == id select i).FirstOrDefault ();
				timeline.SetRepository (this);
				return timeline;
			}
		}
		// temos que colocar 0 na frente quando o mes ou o dia sao menores que 10
		// ex: 2013-1-1 tem que virar 2013-01-01
		string prependZero (int value)
		{
			string valueAsString = "" + value;
			if (valueAsString.Length == 1)
				valueAsString = "0" + valueAsString;

			return valueAsString;
		}
		// convert o DateTime para o formato do sqlite
		string formatToSqliteDate (DateTime date)
		{
			string w = "" + date.Year + "-" + prependZero (date.Month) + "-" + prependZero (date.Day);
			return w;
		}

		public IList<MediaInfo> GetMediaInfoByPeriod(DateTime first, DateTime last, int timelineId, bool onlyMediaDefaultOfTheDay)
		{
			IList<DayInfo> days= (from i in Table<DayInfo>() where i.Date >= first && i.Date <= last && i.TimelineId == timelineId select i).ToList();

			List<MediaInfo> medias = new List<MediaInfo>();
			foreach (DayInfo day in days) {
				day.SetRepository(this);
				if (onlyMediaDefaultOfTheDay)
					medias.Add(day.GetDefaultVideo());
				else
					medias.AddRange(day.GetVideos());
			}

			return medias;
		}	

		public IList<Compilation> GetUserCompilations (int userId)
		{
			return (from i in Table<Compilation> () where i.UserId == userId select i).ToList ();
		}

		public void DeleteCompilation (Compilation compilation)
		{
			this.DeleteItem (compilation);
		}

		public Compilation GetUserCompilation(int userId, int compilationId)
		{
			lock (locker) {
				Compilation compilation =
					(from i in Table<Compilation> () where i.UserId == userId && i.Id == compilationId select i).FirstOrDefault();
				return compilation;
			}
		}

		public IList<T> GetItems<T> () where T : IModel, new()
		{
			lock (locker) {
				return (from i in Table<T> () select i).ToList ();
			}
		}
		// nao sei pq esse metodo nao funciona...
		//public T GetItem<T> (int id) where T : IModel, new ()
		//{
		//    lock (locker) {
		//        return (from i in Table<T> ()
		//                where i.Id == id
		//                select i).FirstOrDefault ();
		//    }
		//}
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
		
		public int DeleteItem<T> (T item) where T : IModel
		{
			lock (locker) {
				return Delete (item);
			}
		}

		public void Reset ()
		{
			// soh permite deletar em debug.. para testes..
#if DEBUG
			this.DeleteAll<User> ();
			this.DeleteAll<Timeline> ();
			this.DeleteAll<DayInfo> ();
			this.DeleteAll<MediaInfo> ();
			this.DeleteAll<Compilation> ();

#endif
		}
	}
}