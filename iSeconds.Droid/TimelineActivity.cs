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
using LegacyBar.Library.BarActions;
using iSeconds.Domain.Framework;

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
	public class TimelineActivity : BaseTimelineActivity, FileObserverNotify
	{
		private CalendarMonthView monthView;

		private const string CurrentDateState= "currenteDateState";
		private const string FirstDateSelected= "firstDateSelected";
		private const string SecondDateSelected= "secondDateSelected";
		private const int ShowOptionsMenu= 101;

		private FileObservadoro fileObservadoro;

		private const int TutorialDialogId = 15;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			this.DisableBackButtonNavigation= true;

			ISecondsApplication application = this.Application as ISecondsApplication;
			IPathService pathService = application.GetPathService();
			fileObservadoro = new FileObservadoro(pathService.GetMediaPath(), this);
			fileObservadoro.StartWatching();

			loadSavedState(bundle);

			viewModel.ShowTutorialCommand.Execute(null);
		}

		#region FileObserverNotify implementation

		public void OnFileCreated()
		{
			RunOnUiThread(
				() => {
					viewModel.Invalidate();
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

				if (savedState.ContainsKey(FirstDateSelected))
					viewModel.RangeSelectionCommand.Execute(DateTime.FromBinary(savedState.GetLong(FirstDateSelected)));

				if (savedState.ContainsKey(SecondDateSelected))
					viewModel.RangeSelectionCommand.Execute(DateTime.FromBinary(savedState.GetLong(SecondDateSelected)));
			}
		}

		protected override bool showHomeButton ()
		{
			return false;
		}

		protected override string getActivityTitle ()
		{
			return this.viewModel.TimelineName;
		}

		protected override void addActionBarItems()
		{
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);

			var takeVideoAction = new ActionLegacyBarAction (this, () => viewModel.TakeVideoCommand.Execute (null), Resource.Drawable.ic_camera);
			takeVideoAction.ActionType = ActionType.Always;
			actionBar.AddAction (takeVideoAction);			
			var menuAction = new ActionLegacyBarAction (this, () => showPopup (), Resource.Drawable.ic_menu);
			menuAction.ActionType = ActionType.Always;
			actionBar.AddAction (menuAction);
		}

		private void setProgressVisibility(bool isVisible)
		{
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);

			if (isVisible)
				actionBar.ProgressBarVisibility = ViewStates.Visible;
			else
				actionBar.ProgressBarVisibility = ViewStates.Gone;
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			showPopup();
			return false;
		}

		private void showPopup()
		{
			TimelineOptionsPopup.Show(this, viewModel);
		}
	}
}

