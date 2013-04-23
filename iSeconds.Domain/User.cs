using System;
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
			CurrentTimelineId = -1;
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

		   if (CurrentTimelineId == -1)
			   CurrentTimeline = timeline;

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

		public event EventHandler<GenericEventArgs<Timeline>> OnCurrentTimelineChanged;

		[IgnoreAttribute]
	   public Timeline CurrentTimeline
	   {
	      get
	      {
		      IList<Timeline> timelines = GetTimelines();
	         foreach (Timeline timeline in timelines)
	         {
		         if (timeline.Id == CurrentTimelineId)
			         return timeline;
	         }

				if (timelines.Count > 0)
				{
					CurrentTimelineId = timelines[0].Id;
					return timelines[0];
				}
					

	         return null;
	      }
			set
			{
				if (value.Id != CurrentTimelineId && value.UserId == this.Id)
				{
					CurrentTimelineId = value.Id;

					if (OnCurrentTimelineChanged != null)
						OnCurrentTimelineChanged(this, new GenericEventArgs<Timeline>(value));
				} 
			}
	   }
      
      public void DeleteTimeline(Timeline timeline)
      {
         if (this.Id == timeline.UserId)
            repository.DeleteTimeline(timeline);
      }

	   #region db

	   public string Name { get; set; }

	   [PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public int CurrentTimelineId { get; set; }

	   #endregion
	}
}