
using System;
using System.Collections.Generic;
using SQLite;

namespace iSeconds.Domain
{
	public class Timeline : iSeconds.Domain.IModel
	{
		public event EventHandler<GenericEventArgs<DayInfo>> OnDayChanged;

		public Timeline (string name, int userId)
		{
			this.Name = name;
			this.UserId = userId;
		}

		public Timeline ()
		{
		}

        // temporariamente comentado.. gostaria que essa api fosse assim e nao orientada a banco de dados
        //public void AddVideoAt(DateTime date, string url)
        //{
        //    if (!days.ContainsKey(date))
        //        days.Add(date, new DayInfo(date, this.Id));

        //    days[date].AddVideo(url);

        //    if (OnDayChanged != null)
        //        OnDayChanged(this, new GenericEventArgs<DayInfo>(days[date]));
        //}

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

		#region db
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public int UserId { get; set; }

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
		#endregion


		// only client side...
		private Dictionary<DateTime, DayInfo> days = new Dictionary<DateTime, DayInfo>();
	}
}

