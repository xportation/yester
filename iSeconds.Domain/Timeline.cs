
using System;
using System.Collections.Generic;
using SQLite;
using System.Diagnostics;

namespace iSeconds.Domain
{
	public class Timeline : iSeconds.Domain.IModel
	{
		public event EventHandler<GenericEventArgs<DayInfo>> OnDayChanged;

        private IRepository repository = null;

		public Timeline (string name, int userId)
		{
			this.Name = name;
			this.UserId = userId;
		}

		public Timeline ()
		{
		}

        public void SetRepository(IRepository repository)
        {
            this.repository = repository;
        }

        public void AddVideoAt(DateTime date, string url)
        {
            Debug.Assert(repository != null); 

            DayInfo day = this.repository.GetDayInfoAt(date, this.Id);
            day.AddVideo(url);
        }

        public IList<MediaInfo> GetVideosAt(DateTime date)
        {
            Debug.Assert(repository != null); 

            DayInfo day = this.repository.GetDayInfoAt(date, this.Id);
            return day.GetVideos();
        }

        public DayInfo GetDayAt(DateTime date)
        {
            Debug.Assert(repository != null); 

            return this.repository.GetDayInfoAt(date, this.Id);
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
	}
}

