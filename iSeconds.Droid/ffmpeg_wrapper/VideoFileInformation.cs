using System;
using System.Text.RegularExpressions;

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

		public int AudioFrequencyInHz()
		{
			var re = new Regex("(\\d{1,7}) Hz,");
			var m = re.Match(this.AudioFormat);

			int frequency = 41000;
			if (m.Success)
				int.TryParse(m.Groups[1].Value, out frequency);

			return frequency;
		}

		public double AudioBitRateInKBits()
		{
			var lastPart = this.AudioFormat.Substring (this.AudioFormat.LastIndexOf (',') + 1).Trim();

			string[] parts = lastPart.Split(' ');

			double bitrate = 128.0;
			if (parts.Length >= 2) 
				double.TryParse (parts [0], out bitrate);

			return bitrate;
		}

		public VideoFileInformation()
		{
		}
	}
}

