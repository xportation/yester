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

		public string LockPath()
		{
			return Path + ".lock";
		}
	}
}

