
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
using CalendarControl;

namespace iSeconds
{
	[Activity (Label = "TimelineActivity")]			
	public class TimelineActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			this.SetContentView(Resource.Layout.Timeline);

			CalendarMonthView calendarView = FindViewById<CalendarMonthView>(Resource.Id.calendarView2);
			calendarView.SetDate(DateTime.Now);
			
			// Handlers tests
			calendarView.OnDateSelect = delegate(DateTime obj) {
				Console.WriteLine("Date: {0}", obj.ToShortDateString());		
			};
			
			calendarView.OnMonthChanged = delegate(DateTime obj) {
				Console.WriteLine("Month changed! New date: {0}", obj.ToShortDateString());		
			};
			
			// Marker test
			//			calendarView.IsDateAvailable = delegate(DateTime arg) {
			//				return ((arg.Day >= 10) && (arg.Day <= 22));
			//			};
			
			// Custom weekend days
			/*Button btnChangeWeekendDays = (Button)FindViewById(Resource.Id.btnChangeWeekendDays);
			btnChangeWeekendDays.Click += delegate(object sender, EventArgs e) {
				useEgyptianWeekend = !useEgyptianWeekend;
				if (useEgyptianWeekend)
					calendarView.WeekendDays = new List<DayOfWeek>() {DayOfWeek.Friday, DayOfWeek.Saturday};
				else
					calendarView.WeekendDays = new List<DayOfWeek>() {DayOfWeek.Saturday, DayOfWeek.Sunday};
				// HighlightWeekend test
				//calendarView.HighlightWeekend = !useEgyptianWeekend;
			};*/

		}
	}
}

