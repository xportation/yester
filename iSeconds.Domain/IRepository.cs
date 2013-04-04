using System;
using System.Collections.Generic;

namespace iSeconds.Domain
{
	public interface IRepository
	{
        event EventHandler<GenericEventArgs<Timeline>> OnNewTimeline;
        event EventHandler<GenericEventArgs<DayInfo>> OnDayChanged;

		User GetUser (int id);
		
		IList<Timeline> GetUserTimelines (int userId);
		
		int SaveItem<T> (T item) where T : IModel;

        void SaveTimeline(Timeline timeline);


        DayInfo GetDayInfoAt(DateTime dateTime, int timelineId);

        Timeline GetUserTimeline(int userId, int timelineId);

		MediaInfo GetMediaById(int id);
		MediaInfo GetMediaByPath (string videopath);
		IList<MediaInfo> GetMediasForDay(DayInfo day);
    }

}

