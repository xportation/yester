
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

namespace iSeconds
{
	[Activity (Label = "InspectDbActivity")]			
	public class InspectDbActivity : Activity
	{
		IRepository persistenceService = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			persistenceService = ((ISecondsApplication)this.Application).GetRepository();

			this.SetContentView (Resource.Layout.InspectDb);

			TextView contentsView = this.FindViewById<TextView> (Resource.Id.contentsText);

			Button usersButton = this.FindViewById<Button>(Resource.Id.usersButton);
			usersButton.Click+= delegate {
				contentsView.Text = "";
				
				IEnumerable<User> users = persistenceService.GetUsers();
				foreach (User user in users)
				{
					contentsView.Append(user.Name);
					contentsView.Append("\n");
				}
			};

			Button timelinesButton = this.FindViewById<Button> (Resource.Id.timelinesButton);
			timelinesButton.Click+= delegate {

				contentsView.Text = "";
				
				IEnumerable<Timeline> timelines = persistenceService.GetTimelines();				
				foreach (Timeline timeline in timelines)
				{
					contentsView.Append(timeline.Name);
					contentsView.Append("\n");
				}
			};

			Button daysButton = this.FindViewById<Button> (Resource.Id.daysButton);
			daysButton.Click+= delegate {

				contentsView.Text = "";

				IEnumerable<DayInfo> days = persistenceService.GetDays();				
				foreach (DayInfo day in days)
				{
					contentsView.Append(day.Date.ToString());
					contentsView.Append(day.TimelineId.ToString());
					contentsView.Append("\n");
				}
			};

		}
	}
}

