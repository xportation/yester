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

			return System.IO.Path.GetDirectoryName(Path) + "/" + System.IO.Path.GetFileNameWithoutExtension(Path) + ".png";
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

