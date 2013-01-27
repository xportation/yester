
using System;
using System.Collections.Generic;


namespace iSeconds.Domain
{
	public class Timeline
	{
		public event EventHandler<GenericEventArgs<DayInfo>> OnDayChanged;

		public Timeline (string name)
		{
			this.Name = name;
		}

		public void AddVideoAt (DateTime date, string url)
		{
			if (!days.ContainsKey(date))
				days.Add(date, new DayInfo());

			days[date].AddVideo(url);

			if (OnDayChanged != null)
				OnDayChanged(this, new GenericEventArgs<DayInfo>(days[date]));
		}

		public bool HasVideoAt (DateTime date)
		{
			return days.ContainsKey(date) && days[date].HasVideo();
		}

		public int GetVideoCountAt (DateTime date)
		{
			if (!days.ContainsKey(date))
				return 0;

			return days[date].GetVideoCount();
		}

		public string GetDayThumbnail (DateTime date)
		{
			if (!days.ContainsKey(date))
				return "";

			return days[date].GetThumbnail();
		}

		private string name;
		public string Name 
		{
			get {
				return name;
			}
			set {
				name = value;
			}
		}

		private Dictionary<DateTime, DayInfo> days = new Dictionary<DateTime, DayInfo>();
	}
}

