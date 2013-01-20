using System.Collections.Generic;
using System;
using iSeconds.Domain;

namespace iSeconds.Domain
{
	public class User
	{
		public event EventHandler<GenericEventArgs<Timeline>> OnNewTimeline;

		public void CreateTimeline (string timelineName)
		{
			Timeline timeline = new Timeline(timelineName);
			timelines.Add(timeline);
			if (OnNewTimeline != null)
				OnNewTimeline(this, new GenericEventArgs<Timeline>(timeline));
		}

		public List<Timeline> GetTimelines ()
		{
			return timelines;
		}

		public int GetTimelineCount ()
		{
			return timelines.Count;
		}

		private List<Timeline> timelines = new List<Timeline>();
	}

}

