using System;

namespace iSeconds.Droid
{
	public class VideoFileInformation
	{
		private string _Path;
		public string Path
		{
			get
			{
				return _Path;
			}
			set
			{
				_Path = value;
			}
		}

		public TimeSpan Duration { get; set; }
		public double BitRate { get; set; }
		public string AudioFormat { get; set; }
		public string VideoFormat { get; set; }
		public int Height { get; set; }
		public int Width { get; set; }
		public double Fps { get; set; }

		public VideoFileInformation()
		{
		}
	}
}

