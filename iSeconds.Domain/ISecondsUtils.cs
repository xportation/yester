using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace iSeconds.Domain
{
	public class ISecondsUtils
	{
		public static string StringifyDate(string prefix, System.DateTime dateTime)
		{
			dateTime = dateTime.Date + DateTime.Now.TimeOfDay; // setting the hour
         return String.Format("{0}__{1}_{2}_{3}__{4}_{5}_{6}", prefix, dateTime.Day, dateTime.Month, dateTime.Year, dateTime.Hour, dateTime.Minute, dateTime.Second);
		}

		public static string DateToString(DateTime date, bool withTimeSpan)
		{
			if (withTimeSpan)
				return date.ToString("g", Thread.CurrentThread.CurrentCulture);

			return date.ToString("d", Thread.CurrentThread.CurrentCulture);
		}

		public static string FileSizeFormated(string filename)
		{
			long bytes = FileSize(filename);
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

		/// <summary>
		/// Files the size.
		/// </summary>
		/// <returns>The file size in bytes</returns>
		/// <param name="filename">Filename.</param>
		public static long FileSize(string filename)
		{
			long bytes = 0;
			FileInfo fileInfo = new FileInfo(filename);
			if (fileInfo.Exists)
				bytes= fileInfo.Length;

			return bytes;
		}
	}
}

