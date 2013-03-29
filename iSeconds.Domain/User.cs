using System.Collections.Generic;
using System;
using iSeconds.Domain;
using System.ComponentModel;
using SQLite;

namespace iSeconds.Domain
{
	public class User : IModel
	{
        private IRepository repository = null;

		public User (string name, IRepository repository)
		{
			this.Name = name;
            this.repository = repository;
		}

		public User ()
		{
		}

		public void CreateTimeline (string timelineName)
		{
            Timeline timeline = new Timeline(timelineName, this.Id, this.repository);

            repository.SaveTimeline(timeline);

            // TODO: implement user preferences
            //this.ActualTimeline = timeline;
		}

        public int GetTimelineCount()
        {
            return this.GetTimelines().Count;
        }

        public IList<Timeline> GetTimelines()
        {
            return repository.GetUserTimelines(this.Id);
        }

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

		#region db
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public string Name { get; set; }
		#endregion

        
    }

}

