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

namespace iSeconds.Droid
{
	[Activity(Label = "SingleShotVideoPlayerActivity")]			
	public class SingleShotVideoPlayerActivity : ISecondsActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			this.RequestWindowFeature(WindowFeatures.NoTitle);
			this.SetContentView (Resource.Layout.SingleShotVideoPlayer);

			string fileName = this.Intent.Extras.GetString("FileName");
			configureActionBar(true, "");

			ImageView playOverImage= FindViewById<ImageView>(Resource.Id.singleShotImagePausePlay);
			VideoView videoView= FindViewById<VideoView>(Resource.Id.singleShotVideoView);
			videoView.SetVideoPath(fileName);
			videoView.Touch+= (videoSender, videoEv) => {
				if ((videoEv.Event.Action & MotionEventActions.Mask) == MotionEventActions.Down) {
					videoView.Start();
					playOverImage.Visibility = ViewStates.Gone;
				}
			};

			videoView.Completion+= (videoSender, videoEv) => playOverImage.Visibility = ViewStates.Visible;

			playOverImage.Visibility = ViewStates.Gone;
			videoView.Start();
		}
	}
}

