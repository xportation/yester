
using System;
using System.Collections.Generic;

namespace iSeconds.Domain
{
	public class DayInfo
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

		public string GetThumbnail ()
		{
			// TODO: temporariamente assim.. teriamos que ver o que vamos retornar
			return !HasVideo() ? "" : videos[0];
		}

		private List<string> videos = new List<string>();
	}

}

