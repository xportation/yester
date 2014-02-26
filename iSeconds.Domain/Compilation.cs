using System;
using SQLite;
using System.IO;

namespace iSeconds.Domain
{
	public class Compilation : IModel
	{
		#region db

		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public string Path { get; set; }
		public string ThumbnailPath { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string TimelineName { get; set; }
		public DateTime Begin { get; set; }
		public DateTime End { get; set; }

		public int UserId { get; set; }
		public bool Done { get; set; }

		#endregion

		public bool IsLost()
		{
			if (this.Done)
				return false;

			if (ISecondsUtils.FileSize(this.Path) == 0 || !File.Exists(this.ThumbnailPath))
				return true;

			return false;
		}
	}
}

