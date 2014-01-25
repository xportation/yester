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
using iSeconds.Domain;
using System.ComponentModel;

namespace iSeconds.Droid
{
	[Activity (Label = "BaseTimelineActivity")]			
	public abstract class BaseTimelineActivity : ISecondsActivity
	{
		protected TimelineViewModel viewModel;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			this.RequestWindowFeature(WindowFeatures.NoTitle);
			this.SetContentView(Resource.Layout.TimelineView);

			ISecondsApplication application = this.Application as ISecondsApplication;
			viewModel = new TimelineViewModel(application.GetUserService().CurrentUser, application.GetRepository(), 
				application.GetMediaService(), application.GetNavigator(), application.GetOptionsDialogService(),
				application.GetI18nService(), application.GetPathService());

			configureActionBar(showHomeButton(), "");
			addActionBarItems();
			setupCalendar();
		}

		protected abstract bool showHomeButton();
		protected abstract void addActionBarItems();
		protected abstract string getActivityTitle ();


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

			CalendarMonthView monthView = FindViewById<CalendarMonthView> (Resource.Id.calendarView);
			monthView.ViewedDays = viewModel.VisibleDays;
			monthView.ViewModel = viewModel;

			this.viewModel.OnRangeSelectionMode = false;

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

				/*if (e.PropertyName == "RangeSelection") {

					if (this.viewModel.OnRangeSelectionMode)
						addActionBarItems(true);
					else
						addActionBarItems(false);
				}*/
			};
		}


		private void setActionBarTitle()
		{
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
			actionBar.Title = getActivityTitle();
		}
	}
}

