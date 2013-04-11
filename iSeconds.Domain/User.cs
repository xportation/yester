using System.Collections.Generic;
using SQLite;

namespace iSeconds.Domain
{
	public class User : IModel
	{
		private readonly IRepository repository;

		public User(string name, IRepository repository)
		{
			Name = name;
			this.repository = repository;
		}

		public User()
		{
		}

	   public Timeline CreateTimeline(string timelineName, string timelineDescription)
		{
			var timeline = new Timeline(timelineName, Id);
	      timeline.Description = timelineDescription;
			timeline.SetRepository(repository);

			repository.SaveTimeline(timeline);

			// TODO: implement user preferences
			//this.ActualTimeline = timeline;

			return timeline;
		}

	   public bool UpdateTimeline(Timeline timeline)
	   {
	      if (timeline.UserId != this.Id)
	         return false;
   
         repository.SaveTimeline(timeline);
	      return true;
	   }

	   public int GetTimelineCount()
		{
			return GetTimelines().Count;
		}

	   public IList<Timeline> GetTimelines()
		{
			return repository.GetUserTimelines(Id);
		}

	   public Timeline GetTimelineById(int timelineId)
		{
			return repository.GetUserTimeline(Id, timelineId);
		}

	   #region db

	   public string Name { get; set; }

	   [PrimaryKey, AutoIncrement]
		public int Id { get; set; }

	   #endregion

	   // TODO: implementar nas user preferences...		

//        private Timeline _ActualTimeline = null;

//        [Ignore]

//        public Timeline ActualTimeline {

//            get {

////				if (_ActualTimeline == null)

////					throw new Exception("Has no actual timeline");

////

//                return _ActualTimeline;

//            }

//            set {

//                _ActualTimeline = value;

//                if (OnActualTimelineChanged != null)

//                    OnActualTimelineChanged(this, new GenericEventArgs<Timeline>(_ActualTimeline));

//            }

//        }
	}
}