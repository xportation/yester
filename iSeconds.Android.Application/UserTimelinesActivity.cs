
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
	[Activity (Label = "UserTimelinesActivity")]			
	public class UserTimelinesActivity : Activity
	{
		int CREATE_TIMELINE_RESULT = 1;
		
		const string TIMELINE_NAME_EXTRA = "TIMELINE_NAME_EXTRA";

		private User user = null;

		private LinearLayout layout = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			this.SetContentView(Resource.Layout.UserTimelines);

			layout = this.FindViewById<LinearLayout> (Resource.Id.userTimelinesLayout);

			Button createTimelineButton = FindViewById<Button> (Resource.Id.createTimeline);
			createTimelineButton.Click += delegate {
				this.StartActivityForResult (typeof(iSeconds.Android.Application.CreateTimelineActivity), CREATE_TIMELINE_RESULT);
			};

			UserService userService = ((ISecondsApplication)this.Application).GetUserService();
			user = userService.GetActualUser();

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
			if (requestCode == CREATE_TIMELINE_RESULT) 
			{
				if (resultCode == Result.Ok) 
				{
					string timelineName = data.GetStringExtra(TIMELINE_NAME_EXTRA);
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
				this.StartActivity (typeof(iSeconds.TimelineActivity));
			};

			layout.AddView (button, ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent);
		}
	}
}

