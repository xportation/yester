using System;
using System.Collections.Generic;

namespace iSeconds.Domain
{
   public interface IRepository
   {
      event EventHandler<GenericEventArgs<Timeline>> OnSaveTimeline;
      event EventHandler<GenericEventArgs<Timeline>> OnDeleteTimeline;

      //User GetUser(int id);

      IList<Timeline> GetUserTimelines(int userId);

      int SaveItem<T>(T item) where T : IModel;

      void SaveTimeline(Timeline timeline);
      void DeleteTimeline(Timeline timeline);
      
      DayInfo GetDayInfoAt(DateTime dateTime, int timelineId);

		DayInfo GetDayInfo(int dayId);

		User GetUser(string userName);
		void SaveUser(User user);

      Timeline GetUserTimeline(int userId, int timelineId);

      MediaInfo GetMediaById(int id);
      MediaInfo GetMediaByPath(string videopath);
      IList<MediaInfo> GetMediasForDay(DayInfo day);
      void DeleteMedia (MediaInfo media);

      IList<string> GetVideosFromRange(DateTime start, DateTime end, int timelineId);
		IList<MediaInfo> GetMediaInfoByPeriod(DateTime first, DateTime last, int timelineId);

		IList<Compilation> GetUserCompilations (int userId);

		void DeleteCompilation (Compilation compilation);

		Compilation GetUserCompilation(int userId, int compilationId);

	}
}
