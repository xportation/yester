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
		private IRepository repository = null;
		private IOptionsDialogService optionsDialog = null;

		private EventHandler<GenericEventArgs<Timeline>> timelineChangedHandler;
		
		public TimelineOptionsViewModel(INavigator navigador, User user, IRepository repository, 
			I18nService i18n, IOptionsDialogService optionsDialog)
		{
			this.user = user;
			this.i18n = i18n;
			this.navigator = navigador;
			this.repository = repository;
			this.optionsDialog = optionsDialog;

			timelineChangedHandler= new EventHandler<GenericEventArgs<Timeline>>((sender, e) => notifyChanges());

			this.user.OnCurrentTimelineChanged += timelineChangedHandler;
			this.user.OnTimelineUpdated += timelineChangedHandler;
			this.repository.OnSaveTimeline += timelineChangedHandler;
			this.repository.OnDeleteTimeline += timelineChangedHandler;
		}

		public void Disconnect()
		{
			this.user.OnCurrentTimelineChanged -= timelineChangedHandler;
			this.user.OnTimelineUpdated -= timelineChangedHandler;
			this.repository.OnSaveTimeline -= timelineChangedHandler;
			this.repository.OnDeleteTimeline -= timelineChangedHandler;
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

		public void DeleteTimeline(Timeline timeline, bool deleteVideosLinked)
	   {
			optionsDialog.ShowProgressDialog(() => {
				user.DeleteTimeline(timeline, deleteVideosLinked);

			   if (user.GetTimelineCount() == 0)
					user.CreateTimeline(i18n.Msg("Default Timeline"), i18n.Msg("Default Timeline"));

				var newCurrentTimeline= this.TimelineAt(0);
				this.SetCurrentTimeline(newCurrentTimeline);
			}, i18n.Msg("Please wait..."));
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

				AddEntry(new OptionsEntry(i18n.Msg("Edit Timeline"), () => viewModel.EditTimelineCommand.Execute(this.currentTimeline)));
				AddEntry(new OptionsEntry(i18n.Msg("Set as current"), () => viewModel.SetCurrentTimeline(this.currentTimeline)));
				AddEntry(new OptionsEntry(i18n.Msg("Delete"), () => viewModel.DeleteTimelineCommand.Execute(this.currentTimeline)));
				AddEntry(new OptionsEntry(i18n.Msg("Cancel"), () => { /*nothing to do*/ }));
			}
		}

		public ICommand TimelineOptionsCommand
		{
			get 
			{ 
				return new Command((object arg) =>
					{
						var currentTimelineInEdition = this.TimelineAt((int) arg);
						optionsDialog.ShowModal(new TimelineOptionsList(this, currentTimelineInEdition, i18n));
					}); 
			}
		}

		#endregion

		public ICommand AddTimelineCommand
		{
			get { return new Command((object arg) => {
				optionsDialog.AskForTimelineNameAndDescription("","",	(string name, string description) => 
					this.AddTimeline(name, description),
					null);
			}); 
			}
		}

		public ICommand EditTimelineCommand
		{
			get { return new Command((object arg) => {
				Timeline currentTimeline = (Timeline) arg;
				optionsDialog.AskForTimelineNameAndDescription(currentTimeline.Name,currentTimeline.Description,
					(string name, string description) => {
						currentTimeline.Name = name;
						currentTimeline.Description = description;
						this.UpdateTimeline(currentTimeline);
					},
					null);
			}); 
			}
		} 
		
		public ICommand DeleteTimelineCommand
		{
			get 
			{ 
				return new Command((object arg) =>
					{
						Timeline currentTimeline = (Timeline) arg;
						optionsDialog.AskForConfirmation(i18n.Msg("Are you sure to delete timeline?"), 
							() => {
								optionsDialog.AskForConfirmation(i18n.Msg("Do you want to delete the videos linked to this timeline?"), 
									() => this.DeleteTimeline(currentTimeline, true), () => this.DeleteTimeline(currentTimeline, false));
							}, 
							() => {}
						);
					}); 
			}
		}

	}
}