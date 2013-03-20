using System;
using System.Collections.Generic;

namespace iSeconds.Domain
{
	public interface IRepository
	{
		IList<User> GetUsers ();
		User GetUser (int id);
		
		IList<Timeline> GetTimelines ();
		
		IList<DayInfo> GetDays ();

		IList<Timeline> GetUserTimelines (int userId);
		
		int SaveItem<T> (T item) where T : IModel;

        void SaveTimeline(Timeline timeline);

		IList<DayInfo> GetDaysInMonth (int timelineId, int month, int year);

        event EventHandler<GenericEventArgs<Timeline>> OnNewTimeline;
        event EventHandler<GenericEventArgs<DayInfo>> OnDayChanged;

        void SaveDay(int timelineId, DateTime day, string videoPath);

        DayInfo GetDayInfoAt(DateTime dateTime, int timelineId);

        Timeline GetUserTimeline(int userId, int timelineId);
    }

}

