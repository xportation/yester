using System;
using System.Collections.Generic;
using SQLite;
using System.IO;
using System.Diagnostics;

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
      
		public void DeleteTimeline(Timeline timeline, bool deleteVideosLinked)
      {
			if (this.Id == timeline.UserId) {
				if (deleteVideosLinked)
					timeline.DeleteAllVideos();

				repository.DeleteTimeline(timeline);
			}
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

		public IList<Compilation> GetCompilations ()
		{
			return repository.GetUserCompilations(this.Id);
		}

		public void AddCompilation (Compilation compilation)
		{
			compilation.UserId = this.Id;
			repository.SaveItem (compilation);
		}

		public void DeleteCompilation (Compilation compilation)
		{
			if (File.Exists(compilation.Path))
				File.Delete(compilation.Path);

			Debug.Assert (!File.Exists(compilation.Path));

			if (File.Exists(compilation.ThumbnailPath))
				File.Delete(compilation.ThumbnailPath);

			Debug.Assert (!File.Exists(compilation.ThumbnailPath));

			repository.DeleteCompilation (compilation);

         if (!compilation.Done) {
            var serviceCompilation = repository.GetServiceCompilation(compilation.Path);
            if (serviceCompilation != null) {
               serviceCompilation.Status = ServiceCompilation.Error;
               repository.SaveItem(serviceCompilation);
            }
         }
		}

		public void DeleteCompilation (string compilationFilename)
		{
			Compilation compilation= repository.GetUserCompilation(this.Id, compilationFilename);
			if (compilation != null)
				this.DeleteCompilation(compilation);
		}

		public void UpdateCompilation(Compilation compilation)
		{
			if (compilation.UserId != this.Id)
				return;

			repository.SaveCompilation(compilation);
		}

		public void SetCompilationDone(string compilationFilename, bool isDone)
		{
			Compilation compilation= repository.GetUserCompilation(this.Id, compilationFilename);
			if (compilation != null) {
				compilation.Done = isDone;
				this.UpdateCompilation(compilation);
			}
		}

		public void RemoveLostCompilations()
		{
			var compilations = this.GetCompilations();
			foreach (Compilation compilation in compilations) {
            if (!compilation.Done) {
               var serviceCompilation = repository.GetServiceCompilation(compilation.Path);
               if (serviceCompilation == null) {
                  this.DeleteCompilation(compilation);
                  continue;
               }

               if (serviceCompilation.Status == ServiceCompilation.Error)
                  this.DeleteCompilation(compilation);
               else if (serviceCompilation.Status == ServiceCompilation.Completed)
                  this.SetCompilationDone(serviceCompilation.Path, true);
            }
			}
		}

		public Compilation GetCompilationById(int id)
		{
			return repository.GetUserCompilation(this.Id, id);
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