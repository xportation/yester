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

		public static bool FileExists(string filename)
		{
			return File.Exists(filename);
		}

		/// <summary>
		/// Determines if the file is locked for the specified filename.
		/// </summary>
		/// <returns><c>true</c> if is file locked; otherwise, <c>false</c>.</returns>
		/// <param name="filename">Filename.</param>
		public static bool IsFileLocked(string filename)
		{
			FileInfo fileInfo = new FileInfo(filename);
			FileStream stream = null;
			try {
				stream = fileInfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
			}
			catch (IOException)
			{
				//the file is unavailable because it is:
				//still being written to
				//or being processed by another thread
				//or does not exist (has already been processed)
				return true;
			}
			finally
			{
				if (stream != null)
					stream.Close();
			}

			//file is not locked
			return false;
		}
	}
}

