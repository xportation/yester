
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
	[Activity (Label = "Olá! Você deve criar ou escolher um timeline", Theme = "@android:style/Theme.Dialog")]			
	public class UserTimelinesActivity : Activity
	{
		private User user = null;

		private LinearLayout layout = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			this.SetContentView(Resource.Layout.UserTimelines);

			layout = this.FindViewById<LinearLayout> (Resource.Id.userTimelinesLayout);

			Button createTimelineButton = FindViewById<Button> (Resource.Id.createTimeline);
			createTimelineButton.Click += delegate {

				this.StartActivityForResult (
					typeof(iSeconds.Android.Application.CreateTimelineActivity), 
					ISecondsConstants.CREATE_TIMELINE_RESULT
				);
			};

			UserService userService = ((ISecondsApplication)this.Application).GetUserService();
			user = userService.ActualUser;

			foreach (Timeline timeline in user.GetTimelines()) 
			{
				CreateTimelineButton(timeline);
			}

			user.OnNewTimeline+= (object sender, GenericEventArgs<Timeline> args) => 
			{
				CreateTimelineButton(args.Value);
			};
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == ISecondsConstants.CREATE_TIMELINE_RESULT) 
			{
				if (resultCode == Result.Ok) 
				{
					string timelineName = data.GetStringExtra(ISecondsConstants.TIMELINE_NAME_EXTRA);
					Toast toast = Toast.MakeText(this, timelineName, ToastLength.Short);
					toast.Show();				
					user.CreateTimeline(timelineName);
				}
			}
		}

		void CreateTimelineButton (Timeline timeline)
		{
			Button button = new Button (this);
			button.Text = timeline.Name;
			button.Click += delegate {
				user.ActualTimeline = timeline;
				this.SetResult(Result.Ok, this.Intent);
				this.Finish();
				//this.StartActivity (typeof(iSeconds.TimelineActivity));
			};

			layout.AddView (button, ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent);
		}
	}
}

