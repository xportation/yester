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

		public event EventHandler<GenericEventArgs<DayInfo>> OnDayChanged;

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

		/* 
		*	Temporario.. estou subindo soh pra ficar no historico
		*/
		public IList<int> GetYearsWithContent ()
		{
			Debug.Assert(repository != null);
			return repository.GetYearsWithContent (Id);
		}

		public IList<int> GetMonthsWithContent (int year)
		{
			Debug.Assert(repository != null);
			return repository.GetMonthsWithContent (Id, year);
		}

	public IList<string> GetVideosFromRange(DateTime start, DateTime end)
	{
		return repository.GetVideosFromRange(start, end, Id);
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