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
			addActionBarItems();
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
		}

		void loadSavedState(Bundle savedState)
		{
			if (savedState != null) {
				if (savedState.ContainsKey(CurrentDateState))
					viewModel.CurrentDate = DateTime.FromBinary(savedState.GetLong(CurrentDateState));
				
				if (savedState.ContainsKey(TakingVideo))
					takingVideo = savedState.GetBoolean(TakingVideo);
			}
		}

		private void addActionBarItems()
		{
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
			setProgressVisibility(takingVideo);

			var takeVideoAction = new MenuItemLegacyBarAction(
				this, this, Resource.Id.actionbar_takeVideo, Resource.Drawable.ic_camera,
				Resource.String.takeVideo)
			{
				ActionType = ActionType.Always
			};			

			var moreAction = new MenuItemLegacyBarAction(
				this, this, Resource.Id.actionbar_more, Resource.Drawable.ic_action_overflow_dark,
				Resource.String.more)
			{
				ActionType = ActionType.Always
			};

			actionBar.AddAction(takeVideoAction);
			actionBar.AddAction(moreAction);
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

			monthView.RangeSelectionMode = false;

			this.viewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
				{
					if (e.PropertyName == "CurrentMonthTitle") {
						monthLabel.Text = this.viewModel.CurrentMonthTitle;
					}

					if (e.PropertyName == "VisibleDays") {
						monthView.ViewedDays = viewModel.VisibleDays;
					}

					if (e.PropertyName == "TimelineName") {
						setActionBarTitle();
					}
				};
		}

		private void setActionBarTitle()
		{
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
			actionBar.Title = this.viewModel.TimelineName;
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
			case Resource.Id.actionbar_more:
				OnSearchRequested ();
				showPopup ();
				return true;
			}

			return base.OnOptionsItemSelected(item);
		}

		private void showPopup()
		{
			LinearLayout moreContentView = (LinearLayout) this.LayoutInflater.Inflate(Resource.Layout.OverflowMenu, null);

			moreContentView.Measure(Android.Views.View.MeasureSpec.MakeMeasureSpec (0, MeasureSpecMode.Unspecified)
			                        , Android.Views.View.MeasureSpec.MakeMeasureSpec (0, MeasureSpecMode.Unspecified));
			PopupWindow popupWindow = new PopupWindow(this);
			popupWindow.SetBackgroundDrawable(new ColorDrawable(this.Resources.GetColor(Resource.Color.white))); // acaba sendo a cor que da a impressao de bordas
			popupWindow.ContentView = moreContentView;
			// xunxo para pegar o ImageView do overflow adicionado como action pelo LegacyBar
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
			var layout = actionBar.FindViewById<LinearLayout>(Resource.Id.actionbar_actions);
			View view = layout.GetChildAt (layout.ChildCount - 1); // pegamos o ultimo... nao consegui fazer de outro jeito..
			ImageView imageView = (ImageView)view;
			imageView.Selected = true;
			popupWindow.ShowAsDropDown (view);

			popupWindow.Touchable = true;
			popupWindow.Focusable = true;
			popupWindow.OutsideTouchable = true;
			popupWindow.DismissEvent += (object sender, EventArgs e) => {
				imageView.Selected = false;
			};

			Button settingsButton = moreContentView.FindViewById<Button>(Resource.Id.main_more_content_settings);
			settingsButton.Click += (object sender, EventArgs e) =>  {

				viewModel.SettingsCommand.Execute(null);
				popupWindow.Dismiss();

			};

			Button timelineOptionsButton = moreContentView.FindViewById<Button>(Resource.Id.main_more_content_timeline_options);
			timelineOptionsButton.Click += (object sender, EventArgs e) => {
				viewModel.OptionsCommand.Execute(null);
				popupWindow.Dismiss();
			};

			Button shareButton = moreContentView.FindViewById<Button>(Resource.Id.main_more_content_share);
			shareButton.Click += (object sender, EventArgs e) => {
				viewModel.ShareCommand.Execute(null);
				popupWindow.Dismiss();
			};

			Button playButton = moreContentView.FindViewById<Button>(Resource.Id.main_more_content_play);
			playButton.Click += (object sender, EventArgs e) => {
				viewModel.PlayCommand.Execute(null);
				popupWindow.Dismiss();
			};

			Button aboutButton = moreContentView.FindViewById<Button>(Resource.Id.main_more_content_about);
			aboutButton.Click += (object sender, EventArgs e) => {
				viewModel.AboutCommand.Execute(null);
				popupWindow.Dismiss();
			};

			popupWindow.Update(moreContentView.MeasuredWidth, moreContentView.MeasuredHeight);
		}
	}
}

