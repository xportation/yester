using System.Collections.Generic;
using System;
using iSeconds.Domain;
using System.ComponentModel;

namespace iSeconds.Domain
{
	public class User
	{
		public event EventHandler<GenericEventArgs<Timeline>> OnNewTimeline;
		public event EventHandler<GenericEventArgs<Timeline>> OnActualTimelineChanged;

		public void CreateTimeline (string timelineName)
		{
			Timeline timeline = new Timeline(timelineName);
			timelines.Add(timeline);
			if (OnNewTimeline != null)
				OnNewTimeline(this, new GenericEventArgs<Timeline>(timeline));

			ActualTimeline = timeline;
		}

		public List<Timeline> GetTimelines ()
		{
			return timelines;
		}

		public int GetTimelineCount ()
		{
			return timelines.Count;
		}

		public int TimelineCount {
			get {
				return timelines.Count;
			}
		}

		private Timeline _ActualTimeline = null;
		public Timeline ActualTimeline {
			get {
//				if (_ActualTimeline == null)
//					throw new Exception("Has no actual timeline");
//
				return _ActualTimeline;
			}
			set {
				_ActualTimeline = value;
				this.OnActualTimelineChanged(this, new GenericEventArgs<Timeline>(_ActualTimeline));
			}
		}

		private List<Timeline> timelines = new List<Timeline>();
	}

}

