
using System;
using System.Collections.Generic;
using SQLite;
using System.Diagnostics;

namespace iSeconds.Domain
{
	public class DayInfo : IModel
	{
        private IRepository repository = null;

		public DayInfo (DateTime date, int timelineId)
		{
			this.Date = date;
			this.TimelineId = timelineId;
		}

		public DayInfo()
		{
		}

        public void AddVideo(string url)
        {
            Debug.Assert(repository != null); // you should bind a repository with SetRepository() method

            // TODO: ver se isso basta.. salvamos o dia apenas se ele tiver video
            this.repository.SaveItem(this);

            MediaInfo media = new MediaInfo(this.Id, url);
            this.repository.SaveItem(media);
        }

        public IList<MediaInfo> GetVideos()
        {
            return this.repository.GetMediasForDay(this);
        }

		public int GetVideoCount ()
		{
			return GetVideos().Count;
		}

		public string GetThumbnail ()
		{
			// TODO: temporariamente assim.. teriamos que ver o que vamos retornar
            IList<MediaInfo> videos = GetVideos();

            return videos.Count == 0 ? "" : videos[0].Path;
		}

        public void SetRepository(IRepository repository)
        {
            this.repository = repository;
        }

		#region db
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public int TimelineId { get; set; }
		public DateTime Date { get; set; }
		#endregion

        
    }

}

