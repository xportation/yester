using System;
using System.Globalization;
using System.IO;

namespace iSeconds.Domain
{
	public class ISecondsUtils
	{
		public static string StringifyDate(string prefix, System.DateTime dateTime)
		{
			dateTime = dateTime.Date + DateTime.Now.TimeOfDay; // setting the hour

			string movieName = prefix + "_" + dateTime.ToString();
			movieName = movieName.Replace("/", "_");
			movieName = movieName.Replace(" ", "_");
			movieName = movieName.Replace(":", "_");
			return movieName;
		}

		public static string DateToString(DateTime date, bool withTimeSpan)
		{
			if (withTimeSpan)
				return String.Format("{0:g}", date);

			return String.Format("{0:d}", date);
		}

		public static string FileSizeFormated(string filename)
		{
			long bytes = 0;
			FileInfo fileInfo = new FileInfo(filename);
			if (fileInfo.Exists)
				bytes= fileInfo.Length;

			const int scale = 1024;
			string[] orders = new string[] {"GB", "MB", "KB", "Bytes"};
			long max = (long)Math.Pow(scale, orders.Length-1);

			foreach(string order in orders)
			{
				if( bytes > max )
					return string.Format("{0:##.##} {1}", decimal.Divide( bytes, max ), order);

				max /= scale;
			}

			return "0 Bytes";
		}
	}
}

