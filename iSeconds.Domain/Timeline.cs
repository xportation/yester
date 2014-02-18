using System;
using System.Collections.Generic;
using System.Diagnostics;
using SQLite;

namespace iSeconds.Domain
{
	public class Timeline : IModel
	{
		private IRepository repository;

	   public Timeline(string name, int userId)
		{
			Name = name;
			UserId = userId;
		}

		public Timeline()
		{
		}

		public void SetRepository(IRepository repository)
		{
			this.repository = repository;
		}

		public void AddVideoAt(DateTime date, string url)
		{
			Debug.Assert(repository != null);

			DayInfo day = repository.GetDayInfoAt(date, Id);
			day.AddVideo(url);
		}

		public void DeleteVideoAt (DateTime date, string url)
		{
			Debug.Assert (repository != null);

			DayInfo day = repository.GetDayInfoAt(date, Id);
			day.DeleteVideo (url);
		}

		public void DeleteAllVideos()
		{
			Debug.Assert (repository != null);

			var days= repository.GetAllDays(this.Id);
			foreach (DayInfo day in days)
				day.DeleteVideos();
		}

		public IList<MediaInfo> GetVideosAt(DateTime date)
		{
			Debug.Assert(repository != null);

			DayInfo day = repository.GetDayInfoAt(date, Id);
			return day.GetVideos();
		}

		public DayInfo GetDayAt(DateTime date)
		{
			Debug.Assert(repository != null);

			return repository.GetDayInfoAt(date, Id);
		}

		#region db

		public int UserId { get; set; }

		public string Name { get; set; }

	   public string Description { get; set; }

	   [PrimaryKey, AutoIncrement]
		public int Id { get; set; }

	   #endregion
	}
}