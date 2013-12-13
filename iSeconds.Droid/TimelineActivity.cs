using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using LegacyBar.Library.Bar;
using iSeconds.Domain;
using System.ComponentModel;
using Android.Graphics.Drawables;

namespace iSeconds.Droid
{
	interface FileObserverNotify
	{
		void OnFileCreated();
	}

	class FileObservadoro : FileObserver
	{
		private FileObserverNotify notifier;

		public FileObservadoro(string path, FileObserverNotify notifier)
			:	base(path, FileObserverEvents.Create)
		{
			this.notifier = notifier;
		}

		public override void OnEvent(FileObserverEvents e, string path)
		{
			if (e == FileObserverEvents.Create)
				notifier.OnFileCreated();
		}
	}

   [Activity (Label = "TimelineActivity")]
	public class TimelineActivity : ISecondsActivity, FileObserverNotify
	{
		private CalendarMonthView monthView;
		private TimelineViewModel viewModel;

		private const string CurrentDateState= "currenteDateState";
		private const string FirstDateSelected= "firstDateSelected";
		private const string SecondDateSelected= "secondDateSelected";
		private const int ShowOptionsMenu= 101;

		private FileObservadoro fileObservadoro;
		private bool takingVideo = false;
		private const string TakingVideo= "TakingVideo";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			this.DisableBackButtonNavigation= true;

			this.RequestWindowFeature(WindowFeatures.NoTitle);
			this.SetContentView(Resource.Layout.TimelineView);

			ISecondsApplication application = (ISecondsApplication) this.Application;
			viewModel = new TimelineViewModel(application.GetUserService().CurrentUser, application.GetRepository(), 
			                                  application.GetMediaService(), application.GetNavigator());

			IPathService pathService = application.GetPathService();
			fileObservadoro = new FileObservadoro(pathService.GetMediaPath(), this);

			loadSavedState(bundle);

			if (takingVideo)
				fileObservadoro.StartWatching();

			configureActionBar(false, "");
			addActionBarItems(false);
			setupCalendar();

			takingVideo = false;
		}

		#region FileObserverNotify implementation

		public void OnFileCreated()
		{
			RunOnUiThread(
				() => {
					viewModel.Invalidate();
					setProgressVisibility(false);
				}
			);
		}

		#endregion

		protected override void OnResume()
		{
			base.OnResume();
			viewModel.Invalidate();
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);

			outState.PutLong(CurrentDateState, viewModel.CurrentDate.ToBinary());
			outState.PutBoolean(TakingVideo, takingVideo);

			if (viewModel.Range.Count > 0) {
				DateTime[] dateTime = new DateTime[viewModel.Range.Count];
				viewModel.Range.CopyTo(dateTime);
				outState.PutLong(FirstDateSelected, dateTime[0].ToBinary());
				if (viewModel.Range.Count == 2)
					outState.PutLong(SecondDateSelected, dateTime[1].ToBinary());
			}
		}

		void loadSavedState(Bundle savedState)
		{
			if (savedState != null) {
				if (savedState.ContainsKey(CurrentDateState))
					viewModel.CurrentDate = DateTime.FromBinary(savedState.GetLong(CurrentDateState));
				
				if (savedState.ContainsKey(TakingVideo))
					takingVideo = savedState.GetBoolean(TakingVideo);

				if (savedState.ContainsKey(FirstDateSelected))
					viewModel.RangeSelectionCommand.Execute(DateTime.FromBinary(savedState.GetLong(FirstDateSelected)));

				if (savedState.ContainsKey(SecondDateSelected))
					viewModel.RangeSelectionCommand.Execute(DateTime.FromBinary(savedState.GetLong(SecondDateSelected)));
			}
		}

		private void addActionBarItems(bool isPlayModeEnabled)
		{
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
			//setProgressVisibility(takingVideo);
			actionBar.RemoveAllActions();

			if (isPlayModeEnabled) {
				actionBar.AddAction(createAction(Resource.Id.actionbar_playVideo, Resource.Drawable.ic_play, Resource.String.play_video));
				actionBar.AddAction(createAction(Resource.Id.actionbar_compile, Resource.Drawable.ic_compile, Resource.String.compile));
			}

			actionBar.AddAction(createAction(Resource.Id.actionbar_takeVideo, Resource.Drawable.ic_camera, Resource.String.takeVideo));
			actionBar.AddAction(createAction(Resource.Id.actionbar_menu, Resource.Drawable.ic_action_overflow_dark, Resource.String.menu));
		}

		private LegacyBar.Library.Bar.LegacyBarAction createAction(int menuId, int drawable, int popupId)
		{
			var action = new MenuItemLegacyBarAction(
				this, this, menuId, drawable, popupId)
			{
				ActionType = ActionType.Always
			};

			return action;
		}

		private void setProgressVisibility(bool isVisible)
		{
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);

			if (isVisible)
				actionBar.ProgressBarVisibility = ViewStates.Visible;
			else
				actionBar.ProgressBarVisibility = ViewStates.Gone;
		}

		private void setupCalendar()
		{
			setActionBarTitle();

			TextView monthLabel = this.FindViewById<TextView> (Resource.Id.calendarMonthName);
			TextViewUtil.ChangeForDefaultFont(monthLabel, this, 18f);
			monthLabel.Text = this.viewModel.CurrentMonthTitle;

			CalendarMonthViewWeekNames monthWeekNames =
				FindViewById<CalendarMonthViewWeekNames> (Resource.Id.calendarWeekDays);
			List<DayViewModel> weekDays = new List<DayViewModel> (viewModel.VisibleDays.GetRange (0, 7));
			monthWeekNames.WeekDays = weekDays;

			monthView = FindViewById<CalendarMonthView> (Resource.Id.calendarView);
			monthView.ViewedDays = viewModel.VisibleDays;
			monthView.ViewModel = viewModel;

			monthView.RangeSelectionMode = true;

			this.viewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) => {
				if (e.PropertyName == "CurrentMonthTitle") {
					monthLabel.Text = this.viewModel.CurrentMonthTitle;
				}

				if (e.PropertyName == "VisibleDays") {
					monthView.ViewedDays = viewModel.VisibleDays;
				}

				if (e.PropertyName == "TimelineName") {
					setActionBarTitle();
				}

				if (e.PropertyName == "RangeSelection") {
					addActionBarItems(viewModel.Range.Count == 2);
				}
			};
		}

		private void setActionBarTitle()
		{
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
			actionBar.Title = this.viewModel.TimelineName;
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			showPopup();
			return false;
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{			
			case Resource.Id.actionbar_takeVideo:
				OnSearchRequested();
				takingVideo = true;
				viewModel.TakeVideoCommand.Execute(null);				
				return true;
			case Resource.Id.actionbar_menu:
				OnSearchRequested();
				showPopup ();
				return true;
			case Resource.Id.actionbar_playVideo:
				OnSearchRequested();
				viewModel.PlaySelectionCommand.Execute(null);
				return true;
			case Resource.Id.actionbar_compile:
				OnSearchRequested();
				viewModel.CompileCommand.Execute(null);
				return true;
			}

			return base.OnOptionsItemSelected(item);
		}

		private void showPopup()
		{
			TimelineOptionsPopup.Show(this, viewModel);
		}
	}
}

