
using System;
using System.Collections.Generic;

namespace iSeconds.Domain
{
	class DayInfo
	{
		public void AddVideo (string url)
		{
			videos.Add(url);
		}

		public bool HasVideo ()
		{
			return videos.Count > 0;
		}

		public int GetVideoCount ()
		{
			return videos.Count;
		}

		private List<string> videos = new List<string>();
	}

}

