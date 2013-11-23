using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LegacyBar.Library.Bar;
using iSeconds.Domain;
using System.ComponentModel;

namespace iSeconds.Droid
{
	class RangeSelectorDialog : AlertDialog
	{
		public RangeSelectorDialog(Context context) 
			: base(context)
		{
			this.SetContentView (Resource.Layout.DatePickerView);
		}
	}

	[Activity (Label = "RangeSelectorByDayActivity")]			
	public class RangeSelectorByDayActivity : ISecondsActivity
	{
		private CalendarMonthView monthView;
		private TimelineViewModel viewModel;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			this.RequestWindowFeature (WindowFeatures.NoTitle);
			this.SetContentView (Resource.Layout.CalendarRangeSelector);

			ISecondsApplication application = (ISecondsApplication) this.Application;
			viewModel = new TimelineViewModel(application.GetUserService().CurrentUser, application.GetRepository(), 
			                                  application.GetMediaService(), application.GetNavigator());

			configureActionBar (true, "");
			addActionBarItems ();
			setupCalendar ();
		}

		private void addActionBarItems ()
		{
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar> (Resource.Id.actionbar);

			var playVideoAction = new MenuItemLegacyBarAction (
				this, this, Resource.Id.actionbar_playVideo, Resource.Drawable.ic_setting,
				Resource.String.play_video) {
				ActionType = ActionType.Always
			};

			actionBar.AddAction (playVideoAction);

			var rangeSelectorAction = new MenuItemLegacyBarAction (
				this, this, Resource.Id.actionbar_rangeSelector, Resource.Drawable.ic_camera,
				Resource.String.range_selector) {
				ActionType = ActionType.Always
			};

			actionBar.AddAction (rangeSelectorAction);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{			
				case Resource.Id.actionbar_playVideo:
					OnSearchRequested ();
					viewModel.PlaySelectionCommand.Execute(null);
					return true;

				case Resource.Id.actionbar_rangeSelector:
					OnSearchRequested ();
					openRangeSelector (); // TODO: extrair para um command
					return true;
			}

			return base.OnOptionsItemSelected(item);
		}

		void openRangeSelector ()
		{
			Dialog dialog = new Dialog (this);
			dialog.SetContentView (Resource.Layout.DatePickerView);
			Button selectButton = dialog.FindViewById<Button> (Resource.Id.selectButton);
			selectButton.Click += (object sender, EventArgs e) => {
				DatePicker beginPicker = dialog.FindViewById<DatePicker>(Resource.Id.dateStartPeriod);
				DatePicker endPicker = dialog.FindViewById<DatePicker>(Resource.Id.dateEndPeriod);

				viewModel.ToggleSelectionDayCommand.Execute(beginPicker.DateTime);
				viewModel.ToggleSelectionDayCommand.Execute(endPicker.DateTime);

				dialog.Dismiss();
				monthView.Invalidate();

				//viewModel.PlaySelectionCommand.Execute(null);
			};

			Button playButton = dialog.FindViewById<Button> (Resource.Id.playButton);
			playButton.Click += (object sender, EventArgs e) => {
				DatePicker beginPicker = dialog.FindViewById<DatePicker>(Resource.Id.dateStartPeriod);
				DatePicker endPicker = dialog.FindViewById<DatePicker>(Resource.Id.dateEndPeriod);

				viewModel.ToggleSelectionDayCommand.Execute(beginPicker.DateTime);
				viewModel.ToggleSelectionDayCommand.Execute(endPicker.DateTime);
				viewModel.PlaySelectionCommand.Execute(null);
			};

			Button cancelButton = dialog.FindViewById<Button> (Resource.Id.cancelButton);
			cancelButton.Click += (object sender, EventArgs e) =>  {
				dialog.Dismiss();
			};

			dialog.Show ();
		}

		private void setupCalendar ()
		{
			TextView monthLabel = this.FindViewById<TextView> (Resource.Id.calendarMonthName);
			TextViewUtil.ChangeForDefaultFont (monthLabel, this, 18f);
			monthLabel.Text = this.viewModel.CurrentMonthTitle;

			CalendarMonthViewWeekNames monthWeekNames =
				FindViewById<CalendarMonthViewWeekNames> (Resource.Id.calendarWeekDays);
			List<DayViewModel> weekDays = new List<DayViewModel> (viewModel.VisibleDays.GetRange (0, 7));
			monthWeekNames.WeekDays = weekDays;

			monthView = FindViewById<CalendarMonthView> (Resource.Id.calendarView);

			monthView.AllowMultiSelection = true;

			monthView.ViewedDays = viewModel.VisibleDays;
			monthView.ViewModel = viewModel;

			this.viewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
			{
				if (e.PropertyName == "CurrentMonthTitle") {
					monthLabel.Text = this.viewModel.CurrentMonthTitle;
				}

				if (e.PropertyName == "VisibleDays") {
					monthView.ViewedDays = viewModel.VisibleDays;
				}

				if (e.PropertyName == "TimelineName") {
					setActionBarTitle ();
				}
			};

		}

		private void setActionBarTitle ()
		{
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar> (Resource.Id.actionbar);
			actionBar.Title = this.viewModel.TimelineName;
		}
	}
}

