
using System;
using System.Collections.Generic;
using SQLite;

namespace iSeconds.Domain
{
	public class DayInfo : IModel
	{
		public DayInfo (DateTime date, int timelineId)
		{
			this.Date = date;
			this.TimelineId = timelineId;
		}

		public DayInfo()
		{
		}

		public void AddVideo (string url)
		{
			videos.Add(url);
		}

		public bool HasVideo ()
		{
			return videos.Count > 0;
		}

		public int GetVideoCount ()
		{
			return videos.Count;
		}

		public string GetThumbnail ()
		{
			// TODO: temporariamente assim.. teriamos que ver o que vamos retornar
			return !HasVideo() ? "" : videos[0];
		}

		private List<string> videos = new List<string>();

		#region db
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public int TimelineId { get; set; }
		public DateTime Date { get; set; }
		#endregion
	}

}

