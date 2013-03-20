
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

		public bool HasVideo ()
		{
			return medias.Count > 0;
		}

		public int GetVideoCount ()
		{
			return medias.Count;
		}

		public string GetThumbnail ()
		{
			// TODO: temporariamente assim.. teriamos que ver o que vamos retornar
            return !HasVideo() ? "" : medias[0].Path;
		}

        public void LoadMedia(IList<MediaInfo> media)
        {
            this.medias.AddRange(media);
        }


        private List<MediaInfo> medias = new List<MediaInfo>();

		#region db
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public int TimelineId { get; set; }
		public DateTime Date { get; set; }
		#endregion
	}

}

