using iSeconds.Domain.Framework;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace iSeconds.Domain
{
	public class HomeViewModel : ViewModel
	{
		private User user = null;
		private IRepository repository = null;
        private IMediaService mediaService = null;
        private INavigator navigator = null;

        public HomeViewModel(User user, IRepository repository, IMediaService mediaService, INavigator navigator)
		{
			this.user = user;
			this.repository = repository;
            this.mediaService = mediaService;
            this.navigator = navigator;

            this.repository.OnNewTimeline += (object sender, GenericEventArgs<Timeline> arg) =>
            {
                this.CurrentTimeline = new TimelineViewModel(arg.Value, repository, mediaService, this.navigator);
                Timelines = this.repository.GetUserTimelines(this.user.Id);
            };

            Timelines = this.repository.GetUserTimelines(this.user.Id);

            if (Timelines.Count == 0)
            {
                CurrentTimeline = null;
            }
		}
		
		public IList<Timeline> Timelines { get; set; }
		
		private TimelineViewModel currentTimeline;
		public TimelineViewModel CurrentTimeline 
		{
			get { return currentTimeline; }
			set { 
				currentTimeline = value;
                this.SetField(ref currentTimeline, value, "CurrentTimeline");
			}
		}
		
		public ICommand NewTimelineCommand
		{
			get
			{
				return new Command(delegate(object arg)
				{
                    repository.SaveTimeline(new Timeline("xou da xuxa", user.Id, repository));

                    //Timeline timeline = new Timeline("my timeline", this.user.Id);
                    //this.repository.SaveItem(timeline);
                    //CurrentTimeline = new TimelineViewModel(timeline, this.repository);
				});
			}
		}

        public ICommand LoadTimelineCommand
        {
            get
            {
                return new Command(delegate(object arg)
                {
                    int timelineId = (int)arg;
                    CurrentTimeline = new TimelineViewModel(repository.GetUserTimeline(this.user.Id, timelineId), repository, this.mediaService, this.navigator);
                });
            }
        }

    }

}

