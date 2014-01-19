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
using Android.Graphics;
using Android.Graphics.Drawables;

namespace iSeconds.Droid
{
	[Activity(Label = "SingleShotVideoPlayerActivity")]			
	public class SingleShotVideoPlayerActivity : ISecondsActivity
	{
		private bool usesController= false;
		private ImageView playOverImage= null;

		private VideoView videoView;
		private int lastVideoPosition = 0;
		private bool startingPaused = false;

		private const string VideoPaused= "videoPaused";
		private const string VideoPosition= "videoPosition";

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			this.RequestWindowFeature(WindowFeatures.NoTitle);
			this.SetContentView(Resource.Layout.SingleShotVideoPlayer);

			string fileName = this.Intent.Extras.GetString("FileName");
			configureActionBar(true, "");

			playOverImage = FindViewById<ImageView>(Resource.Id.singleShotImagePausePlay);
			videoView = FindViewById<VideoView>(Resource.Id.singleShotVideoView);
			videoView.SetVideoPath(fileName);

			videoView.Completion += (videoSender, videoEv) => setPlayButtonVisibility(true);

			if (this.Intent.Extras.ContainsKey("UsesController"))
				usesController = Boolean.Parse(this.Intent.Extras.GetString("UsesController"));

			if (usesController) {
				MediaController mediaController = new MediaController(this);
				mediaController.SetAnchorView(videoView);
				videoView.SetMediaController(mediaController);
			} else {
				videoView.Touch += (videoSender, videoEv) => {
					if ((videoEv.Event.Action & MotionEventActions.Mask) == MotionEventActions.Down) {
						if (!videoView.IsPlaying) {
							videoView.Start();
							setPlayButtonVisibility(false);
						}
					}
				};
			}

			loadState(savedInstanceState);
			setPlayButtonVisibility(startingPaused);

			videoView.SeekTo(lastVideoPosition);

			if (!startingPaused)
				videoView.Start();
		
			startingPaused= false;
		}

		private void loadState(Bundle bundle)
		{
			if (bundle != null) {
				if (bundle.ContainsKey(VideoPaused))
					startingPaused = bundle.GetBoolean(VideoPaused);

				if (bundle.ContainsKey(VideoPosition))
					lastVideoPosition = bundle.GetInt(VideoPosition);
			}
		}

		private void setPlayButtonVisibility(bool isVisible)
		{
			if (isVisible && !usesController) 
				playOverImage.Visibility = ViewStates.Visible;
			else
				playOverImage.Visibility = ViewStates.Gone;
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
			startingPaused = !videoView.IsPlaying;
			outState.PutBoolean(VideoPaused, startingPaused);

			if (videoView.CurrentPosition < videoView.Duration) {
				lastVideoPosition = videoView.CurrentPosition;
				outState.PutInt(VideoPosition, lastVideoPosition);
			}
		}
	}
}

