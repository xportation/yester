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
using System.Timers;

namespace iSeconds.Droid
{

	[Activity (Label = "TimelineActivity")]
	public class TimelineActivity : BaseTimelineActivity
	{
		private const string CurrentDateState= "currenteDateState";
		private const string FirstDateSelected= "firstDateSelected";
		private const string SecondDateSelected= "secondDateSelected";
		private const int ShowOptionsMenu= 101;

		private Timer singleshotUpdateView = null;

		private const int TutorialDialogId = 15;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			this.DisableBackButtonNavigation= true;

			ISecondsApplication application = this.Application as ISecondsApplication;
			IPathService pathService = application.GetPathService();

			loadSavedState(bundle);

			singleshotUpdateView = new Timer(5000);
			singleshotUpdateView.Elapsed += (sender, e) => {
				singleshotUpdateView.Stop();
				setProgressVisibility(false);
			};

			IMediaService mediaService = application.GetMediaService();
			mediaService.OnVideoRecorded += (sender, e) => {
				setProgressVisibility(true);
				singleshotUpdateView.Start();
			};

			mediaService.OnThumbnailSaved += (sender, e) => {
				viewModel.Invalidate();
				setProgressVisibility(false);
			};
		}

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
			RunOnUiThread(delegate {
				var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);

				if (isVisible) {
					if (actionBar.ProgressBarVisibility == ViewStates.Gone)
						actionBar.ProgressBarVisibility = ViewStates.Visible;
				} else {
					if (actionBar.ProgressBarVisibility == ViewStates.Visible)
						actionBar.ProgressBarVisibility = ViewStates.Gone;
				}
			});
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

