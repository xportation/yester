using System;
using System.IO;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Widget;
using Android.OS;
using iSeconds.Domain;
using Android.Content;

namespace iSeconds
{
	[Activity (Label = "iSeconds", MainLauncher = true, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.Orientation)]
	public class MainActivity : Activity
	{
		// um usuario de teste
		User userTest = new User();

		int CREATE_TIMELINE_RESULT = 1;

		const string TIMELINE_NAME_EXTRA = "TIMELINE_NAME_EXTRA";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			Button createTimelineButton = FindViewById<Button> (Resource.Id.createTimeline);
			createTimelineButton.Click += delegate {
				this.StartActivityForResult (typeof(iSeconds.Android.Application.CreateTimelineActivity), CREATE_TIMELINE_RESULT);
			};

			Button mediaSandboxButton = FindViewById<Button>(Resource.Id.mediaSandboxButton);
			mediaSandboxButton.Click += delegate {
				this.StartActivity(typeof(iSeconds.MediaSandboxActivity));
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
					// TODO: parei aqui
					//userTest.CreateTimeline(timelineName);
				}

			}
		}

	
	
	}

		
}