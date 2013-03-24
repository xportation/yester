using System;
using SQLite;

namespace iSeconds.Domain
{
	public class MediaInfo : IModel
	{
		public MediaInfo (int dayId, string path) 
		{
			this.DayId = dayId;
			this.Path = path;
		}

        public MediaInfo()
        {
        }

		#region db
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public int DayId { get; set; }
		// TODO: ver o tamanho para essa string
		public string Path { get; set; }
		#endregion
	}
}
