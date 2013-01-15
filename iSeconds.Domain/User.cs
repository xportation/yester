using System.Collections.Generic;

namespace iSeconds.Domain
{
	public class User
	{
		public void CreateTimeline ()
		{
			timelines.Add(new Timeline());
		}

		public int GetTimelineCount ()
		{
			return timelines.Count;
		}

		private List<Timeline> timelines = new List<Timeline>();
	}

}

