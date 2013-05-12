using System;
using SQLite;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace iSeconds.Domain
{
   public class ISecondsDB : SQLiteConnection, IRepository
   {
      private static object locker = new object();

      public ISecondsDB(string path)
         : base(path)
      {
         Console.WriteLine("------------------------");
         Console.WriteLine(path);
         Console.WriteLine("------------------------");

         CreateTable<Timeline>();
         CreateTable<User>();
         CreateTable<DayInfo>();
         CreateTable<MediaInfo>();
      }

      public event EventHandler<GenericEventArgs<Timeline>> OnSaveTimeline;
	   public event EventHandler<GenericEventArgs<Timeline>> OnDeleteTimeline;
	   public event EventHandler<GenericEventArgs<DayInfo>> OnDayChanged;

      public void SaveTimeline(Timeline timeline)
      {
         this.SaveItem(timeline);

         if (OnSaveTimeline != null)
            OnSaveTimeline(this, new GenericEventArgs<Timeline>(timeline));
      }
      
      public void DeleteTimeline(Timeline timeline)
      {
         this.DeleteItem(timeline);

	      if (OnDeleteTimeline != null)
		      OnDeleteTimeline(this, new GenericEventArgs<Timeline>(timeline));
      }

      public IList<MediaInfo> GetMediasForDay(DayInfo day)
      {
         return (from i in Table<MediaInfo>() where i.DayId == day.Id select i).ToList();
      }

      public DayInfo GetDayInfoAt(DateTime dateTime, int timelineId)
      {
         DayInfo dayInfo =
            (from i in Table<DayInfo>() where i.TimelineId == timelineId && i.Date == dateTime select i).FirstOrDefault();
         if (dayInfo == null)
         {
            dayInfo = new DayInfo(dateTime, timelineId);
         }
         dayInfo.SetRepository(this);

         return dayInfo;
      }

      public MediaInfo GetMediaById(int id)
      {
         lock (locker)
         {
            return (from i in Table<MediaInfo>() where i.Id == id select i).FirstOrDefault();
         }
      }

      public MediaInfo GetMediaByPath(string videopath)
      {
         lock (locker)
         {
            return (from i in Table<MediaInfo>() where i.Path == videopath select i).FirstOrDefault();
         }
      }

		public User GetUser (string userName)
		{
			lock (locker)
			{
				try {
					User user = (from i in Table<User>() where i.Name == userName select i).First();
					user.SetRepository(this);
					return user;
				} catch (Exception exception) {
					Console.WriteLine (exception.Message);
				}
				return null;
			}
		}

		public void SaveUser(User user)
		{
			this.SaveItem(user);
		}

      public IList<Timeline> GetUserTimelines(int userId)
      {
         lock (locker)
         {
            IList<Timeline> timelines = (from i in Table<Timeline>() where i.UserId == userId select i).ToList();
            foreach (Timeline timeline in timelines)
            {
               timeline.SetRepository(this);
            }
            //return (from i in Table<Timeline>() where i.UserId == userId select i).ToList();
            return timelines;
         }
      }

      public Timeline GetUserTimeline(int userId, int timelineId)
      {
         lock (locker)
         {
            Timeline timeline =
               (from i in Table<Timeline>() where i.UserId == userId && i.Id == timelineId select i).FirstOrDefault();
            timeline.SetRepository(this);
            return timeline;
         }
      }

      public IList<T> GetItems<T>() where T : IModel, new()
      {
         lock (locker)
         {
            return (from i in Table<T>() select i).ToList();
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

      public int SaveItem<T>(T item) where T : IModel
      {
         lock (locker)
         {
            if (item.Id != 0)
            {
               Update(item);
               return item.Id;
            }
            else
            {
               return Insert(item);
            }
         }
      }

      public int DeleteItem<T>(T item) where T : IModel
      {
         lock (locker)
         {
            return Delete(item);
         }
      }

      public void Reset()
      {
         // soh permite deletar em debug.. para testes..
#if DEBUG
         this.DeleteAll<User>();
         this.DeleteAll<Timeline>();
         this.DeleteAll<DayInfo>();
         this.DeleteAll<MediaInfo>();

#endif
      }
   }
}