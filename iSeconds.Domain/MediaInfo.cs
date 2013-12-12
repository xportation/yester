using System;
using SQLite;

namespace iSeconds.Domain
{
	public class MediaInfo : IModel
	{
		public MediaInfo (int dayId, string path, TimeSpan timeOfDay) 
		{
			this.DayId = dayId;
			this.Path = path;
			this.TimeOfDay = timeOfDay;
		}

     	public MediaInfo()
     	{
     	}

		public string GetThumbnailPath()
		{
			if (Path.Length == 0)
				return "";

			string fileName = System.IO.Path.ChangeExtension(this.Path, ".png");
			return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), fileName);
		}

		#region db
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public int DayId { get; set; }
		// TODO: ver o tamanho para essa string
		public string Path { get; set; }
		public TimeSpan TimeOfDay { get; set; }
		#endregion
	}
}

