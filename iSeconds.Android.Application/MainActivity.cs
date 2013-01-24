using System;
using System.IO;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Widget;
using Android.OS;
using iSeconds.Domain;
using Android.Content;
using Android.Views;

namespace iSeconds
{
	[Activity (Label = "iSeconds", MainLauncher = true, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.Orientation)]
	public class MainActivity : ISecondsActivity
	{
		UserService userService = null;
		User actualUser = null;
		LinearLayout layout = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			layout = (LinearLayout)this.FindViewById(Resource.Id.mainLayout);

			userService = ((ISecondsApplication)this.Application).GetUserService ();
			actualUser = userService.ActualUser;
			actualUser.OnActualTimelineChanged+= (object sender, GenericEventArgs<Timeline> e) => {
				invalidateTimeline();
			};
			
			if (actualUser != null) 
			{
				if (actualUser.TimelineCount == 0)
				{
					this.StartActivityForResult(typeof(iSeconds.UserTimelinesActivity), ISecondsConstants.TIMELINE_CHOOSER_RESULT);

				} else {

					invalidateTimeline();

				}

			} 
			//else redirecionar para login...
		}

		TimelineView actualView = null;
		void invalidateTimeline ()
		{
			Timeline timeline = actualUser.ActualTimeline;
			if (actualView == null || actualView.Timeline != timeline) 
			{
				layout.RemoveView(actualView);
				actualView = new TimelineView (timeline, this);
				layout.AddView (actualView);
			}
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == ISecondsConstants.TIMELINE_CHOOSER_RESULT) 
			{
				if (resultCode == Result.Ok) 
				{
					Toast toast = Toast.MakeText(this, actualUser.ActualTimeline.Name, ToastLength.Short);
					toast.Show();

					//invalidateTimeline();
				}
			}
		}


	}

	class TimelineView : LinearLayout
	{
		private Timeline timeline = null;
		public Timeline Timeline {
			get {
				return timeline;
			}
		}

		public TimelineView (Timeline model, Context context)
			: base(context, null)
		{
			timeline = model;

			View.Inflate(context, Resource.Layout.Timeline, this);
		}
	}
	

		
}