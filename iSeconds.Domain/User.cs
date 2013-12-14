using System;
using System.Collections.Generic;
using SQLite;

namespace iSeconds.Domain
{
	public class User : IModel
	{
		private IRepository repository;
		
		public User(string name, IRepository repository)
		{
			Name = name;
			CurrentTimelineId = -1;
			RecordDuration = 3;
			TutorialShown = false;
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

		public void SetRepository (IRepository repository)
		{
			this.repository = repository;
		}

		public event EventHandler<GenericEventArgs<Timeline>> OnTimelineUpdated;

	   public bool UpdateTimeline(Timeline timeline)
	   {
	      if (timeline.UserId != this.Id)
	         return false;
   
         repository.SaveTimeline(timeline);

			if (OnTimelineUpdated != null)
				OnTimelineUpdated(this, new GenericEventArgs<Timeline>(timeline));

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
					repository.SaveUser(this);

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

		/// <summary>
		/// Sets the record duration and save in database.
		/// </summary>
		/// <param name="duration">Duration.</param>
		public void SetRecordDuration(int duration)
		{
			this.RecordDuration = duration;
			repository.SaveUser(this);
		}

		/// <summary>
		/// Sets to use only default video.
		/// </summary>
		/// <param name="onlyDefaultVideo">onlyDefaultVideo.</param>
		public void SetUsesOnlyDefaultVideo(bool onlyDefaultVideo)
		{
			this.UsesOnlyDefaultVideo = onlyDefaultVideo;
			repository.SaveUser(this);
		}

		/// <summary>
		/// Sets the tutorial shown.
		/// </summary>
		/// <param name="shown">shown.</param>
		public void SetTutorialShown(bool shown)
		{
			this.TutorialShown = shown;
			repository.SaveUser(this);
		}

	   #region db

	   public string Name { get; set; }

	   [PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public int CurrentTimelineId { get; set; }

		public int RecordDuration { get; set; }

		public bool UsesOnlyDefaultVideo { get; set; }

		public bool TutorialShown { get; set; }

	   #endregion
	}
}