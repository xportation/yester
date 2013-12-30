using System;

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
	}
}

