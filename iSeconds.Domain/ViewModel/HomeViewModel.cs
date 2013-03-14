using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace iSeconds.Domain
{
	public class HomeViewModel : ViewModel
	{
		private User user = null;
		private IRepository repository = null;
		
		public HomeViewModel(User user, IRepository repository)
		{
			this.user = user;
			this.repository = repository;

            this.repository.OnNewTimeline += (object sender, GenericEventArgs<Timeline> arg) =>
            {
                this.CurrentTimeline = new TimelineViewModel(arg.Value, repository);
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
                    repository.SaveTimeline(new Timeline("xou da xuxa", user.Id));

                    //Timeline timeline = new Timeline("my timeline", this.user.Id);
                    //this.repository.SaveItem(timeline);
                    //CurrentTimeline = new TimelineViewModel(timeline, this.repository);
				});
			}
		}

    }

}

