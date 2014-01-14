using System;
using System.Windows.Input;
using iSeconds.Domain.Framework;

namespace iSeconds.Domain
{
	public class TimelineOptionsViewModel: ViewModel
	{
		private User user = null;
		private I18nService i18n = null;
		private INavigator navigator = null;
		
		public TimelineOptionsViewModel(INavigator navigador, User user, IRepository repository, I18nService i18n)
		{
			this.user = user;
			this.i18n = i18n;
			this.navigator = navigador;
			
	      this.user.OnCurrentTimelineChanged += (sender, args) => notifyChanges();
	      repository.OnSaveTimeline += (sender, args) => notifyChanges();
	      repository.OnDeleteTimeline += (sender, args) => notifyChanges();
		}

		public event EventHandler<GenericEventArgs<TimelineOptionsViewModel>> OnTimelineOptionsViewModelChanged;

		private void notifyChanges()
		{
			if (OnTimelineOptionsViewModelChanged != null)
				OnTimelineOptionsViewModelChanged(this, new GenericEventArgs<TimelineOptionsViewModel>(this));
		}

		public ICommand BackToHomeCommand
		{
			get { return new Command((object arg) => { navigator.NavigateBack(); }); }
		}

		public int TimelinesCount()
		{
			return user.GetTimelineCount();
		}

	   public Timeline TimelineAt(int position)
	   {
			if (position >= 0 && position < user.GetTimelineCount())
				return user.GetTimelines()[position];

		   return null;
	   }

      public void UpdateTimeline(Timeline timeline)
      {
         user.UpdateTimeline(timeline);
      }

      public void AddTimeline(string name, string description)
      {
         user.CreateTimeline(name, description);
      }

	   public void DeleteTimeline(Timeline timeline)
	   {
         user.DeleteTimeline(timeline);

		   if (user.GetTimelineCount() == 0)
				user.CreateTimeline(i18n.Msg("Default Timeline"), i18n.Msg("Default Timeline"));

			var newCurrentTimeline= this.TimelineAt(0);
			this.SetCurrentTimeline(newCurrentTimeline);
	   }

		public void SetCurrentTimeline(Timeline timeline)
		{
			user.CurrentTimeline = timeline;
		}

		public bool IsCurrentTimeline(Timeline timeline)
		{
			return user.CurrentTimelineId == timeline.Id;
		}

		#region Timeline Options

		public class TimelineOptionsList : OptionsList
		{
			private I18nService i18n = null;
			private Timeline currentTimeline;

			public TimelineOptionsList(TimelineOptionsViewModel viewModel, Timeline currentTimeline, I18nService i18n)
			{
				this.i18n = i18n;
				this.currentTimeline = currentTimeline;

				AddEntry(new OptionsEntry(i18n.Msg("Edit Timeline"), 
					() => 
						viewModel.TimelineEditionRequest.Raise(new TimelineEditionModel(viewModel,this.currentTimeline))
				));

				AddEntry(new OptionsEntry(i18n.Msg("Set as current"), () => viewModel.SetCurrentTimeline(this.currentTimeline)));
				AddEntry(new OptionsEntry(i18n.Msg("Delete"), () => viewModel.TimelineDeleteRequest.Raise(new TimelineDeleteModel(viewModel, this.currentTimeline))));
				AddEntry(new OptionsEntry(i18n.Msg("Cancel"), () => { /*nothing to do*/ }));
			}
		}

		private InteractionRequest<TimelineOptionsList> timelineOptionsRequest = new InteractionRequest<TimelineOptionsList>();

		public InteractionRequest<TimelineOptionsList> TimelineOptionsRequest
		{
			get { return timelineOptionsRequest; }
		}

		public ICommand TimelineOptionsCommand
		{
			get 
			{ 
				return new Command((object arg) =>
					{
						var currentTimelineInEdition = this.TimelineAt((int) arg);
						TimelineOptionsRequest.Raise(new TimelineOptionsList(this, currentTimelineInEdition, i18n));
					}); 
			}
		}

		public ICommand SetDefaultCommand
		{
			get 
			{ 
				return new Command((object arg) =>
				                   {
					var currentTimelineInEdition = this.TimelineAt((int) arg);
					this.SetCurrentTimeline(currentTimelineInEdition);
				}); 
			}
		}

		#endregion

		#region Timeline Edition
		
		public class TimelineEditionModel
		{
			private Timeline currentTimeline = null;
			private TimelineOptionsViewModel viewModel;

			public TimelineEditionModel(TimelineOptionsViewModel viewModel, Timeline currentTimeline)
			{
				this.viewModel = viewModel;
				this.currentTimeline = currentTimeline;

				if (currentTimeline != null)
				{
					TimelineName = currentTimeline.Name;
					TimelineDescription = currentTimeline.Description;
				}
				else
				{
					TimelineName = string.Empty;
					TimelineDescription = string.Empty;
				}
			}

			public string TimelineName { get; set; }

			public string TimelineDescription { get; set; }

			public void EditingFinished()
			{
				if (currentTimeline == null)
					viewModel.AddTimeline(TimelineName, TimelineDescription);
				else
				{
					currentTimeline.Name = TimelineName;
					currentTimeline.Description = TimelineDescription;
					viewModel.UpdateTimeline(currentTimeline);
				}
			}
		}

		private InteractionRequest<TimelineEditionModel> timelineEditionRequest = new InteractionRequest<TimelineEditionModel>();
		
		public InteractionRequest<TimelineEditionModel> TimelineEditionRequest
		{
			get { return timelineEditionRequest; }
		}

		private InteractionRequest<TimelineEditionModel> timelineAdditionRequest = new InteractionRequest<TimelineEditionModel>();

		public InteractionRequest<TimelineEditionModel> TimelineAdditionRequest
		{
			get { return timelineAdditionRequest; }
		}

		public ICommand AddTimelineCommand
		{
			get { return new Command((object arg) => { TimelineAdditionRequest.Raise(new TimelineEditionModel(this,null)); }); }
		}

		#endregion 

		#region Timeline Delete

		public class TimelineDeleteModel
		{
			private Timeline currentTimeline = null;
			private TimelineOptionsViewModel viewModel;

			public TimelineDeleteModel(TimelineOptionsViewModel viewModel, Timeline currentTimeline)
			{
				this.viewModel = viewModel;
				this.currentTimeline = currentTimeline;
			}
			
			public void DeletingFinished()
			{
				viewModel.DeleteTimeline(currentTimeline);
			}
		}

		private InteractionRequest<TimelineDeleteModel> timelineDeleteRequest = new InteractionRequest<TimelineDeleteModel>();

		public InteractionRequest<TimelineDeleteModel> TimelineDeleteRequest
		{
			get { return timelineDeleteRequest; }
		}
		
		#endregion
	}
}