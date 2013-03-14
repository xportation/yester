using System.Collections.Generic;
using System;
using iSeconds.Domain;
using System.ComponentModel;
using SQLite;

namespace iSeconds.Domain
{
	public class User : IModel
	{
		public event EventHandler<GenericEventArgs<Timeline>> OnNewTimeline;
		public event EventHandler<GenericEventArgs<Timeline>> OnActualTimelineChanged;

		public User (string name)
		{
			this.Name = name;
		}

		public User ()
		{
		}

		public void CreateTimeline (string timelineName)
		{
			Timeline timeline = new Timeline(timelineName, this.Id);
			timelines.Add(timeline);

			if (OnNewTimeline != null)
				OnNewTimeline(this, new GenericEventArgs<Timeline>(timeline));

			this.ActualTimeline = timeline;
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

		public void LoadTimelines (IEnumerable<Timeline> timelines)
		{
			this.timelines.Clear ();
			this.timelines.AddRange (timelines);
		}


		private Timeline _ActualTimeline = null;
		[Ignore]
		public Timeline ActualTimeline {
			get {
//				if (_ActualTimeline == null)
//					throw new Exception("Has no actual timeline");
//
				return _ActualTimeline;
			}
			set {
				_ActualTimeline = value;
				if (OnActualTimelineChanged != null)
					OnActualTimelineChanged(this, new GenericEventArgs<Timeline>(_ActualTimeline));
			}
		}

		private List<Timeline> timelines = new List<Timeline>();


		#region db
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public string Name { get; set; }
		#endregion

        
    }

}

