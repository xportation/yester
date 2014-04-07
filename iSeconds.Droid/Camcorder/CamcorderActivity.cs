using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Views;
using Android.Util;
using Android.Widget;
using iSeconds.Domain;
using Java.Lang;

namespace iSeconds.Droid
{
	[Activity (Label = "Yester Camcorder", Theme = "@android:style/Theme.NoTitleBar.Fullscreen", 
		ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation)]
	public class CamcorderActivity : ISecondsActivity
	{
		bool isRecording = false;
		CamcorderView camcorderView = null;
		string videoPath;

		private IMediaService mediaService = null;
		private UserService userService = null;

		const int PREVIEW_RESULT = 1;

		private TextView countDownText = null;
		private RadioGroup durationGroup = null;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.Camcorder);

			configureServicesAndParameters ();
			configureCamcorderView ();
			configureRecordButton ();
			configureResolutionDropdown ();
			configureCountdown ();
			configureDurationGroup ();

		}     

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data) {

			if (requestCode == PREVIEW_RESULT) {

				switch (resultCode) {
				case Result.Ok: 
					mediaService.CommitVideo (videoPath);
					this.Finish ();
					break;

				case Result.Canceled:
					deleteVideo (videoPath);
					mediaService.RevertVideo ();
					this.Finish ();
					break;

				case CamcorderPreview.RetakeResult:
					deleteVideo (videoPath);
					resetCountDown ();
					break;				
				
				}
			}
		}

		void deleteVideo(string videoPath)
		{
			Java.IO.File file = new Java.IO.File (videoPath);
			if (file.Exists ()) {
				file.Delete ();
			}
		}

		void configureRecordButton ()
		{
			var recordButton = FindViewById<ImageButton> (Resource.Id.camcorderRecordButton);
			recordButton.Click += (sender, e) =>  {
				if (!isRecording) {
					isRecording = true;
					int duration = getDuration ();
					camcorderView.StartRecording (videoPath, duration);
				}
				else {
					// TODO: implement pause
					//camcorderView.StopRecording ();
					//StartActivity (typeof(CamcorderPreview));
				}
			};
		}

		void configureCountdown ()
		{
			countDownText = (TextView)this.FindViewById (Resource.Id.camcorderCountdown);

			resetCountDown ();

			camcorderView.OnSecondChange += (object sender, EventArgs e) =>  {
				GenericEventArgs<int> args = (GenericEventArgs<int>)e;
				this.RunOnUiThread (() =>  {
					countDownText.Text = "00:0" + args.Value;
				});
			};
		}

		void configureResolutionDropdown ()
		{
			RelativeLayout layout = FindViewById<RelativeLayout> (Resource.Id.layoutCamcorder);

			ArrayAdapter<string> adapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleSpinnerItem);
			adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			var spinner = FindViewById<Spinner> (Resource.Id.camcorderResolution);
			spinner.Adapter = adapter;

			spinner.ItemSelected += (s, evt) =>  {
				Android.Graphics.Rect rect = new Android.Graphics.Rect ();
				layout.GetDrawingRect (rect);
				camcorderView.SetPreviewSize (evt.Position, rect.Width (), rect.Height ());
			};

			camcorderView.OnCameraReady += (object sender, EventArgs e) =>  {
				adapter.Clear ();
				foreach (Camera.Size size in camcorderView.CameraSizes)
					adapter.Add (size.Width.ToString () + " x " + size.Height.ToString ());
			};
		}

		void configureCamcorderView ()
		{
			RelativeLayout layout = FindViewById<RelativeLayout> (Resource.Id.layoutCamcorder);
			camcorderView = new CamcorderView (this, 0);
			ViewGroup.LayoutParams camcorderLayoutParams = new ViewGroup.LayoutParams (ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
			layout.AddView (camcorderView, 0, camcorderLayoutParams);
			camcorderView.OnVideoRecorded += (object sender, EventArgs e) =>  {
				isRecording = false;
				Intent intent = new Intent (this, typeof(CamcorderPreview));
				Bundle bundle = new Bundle ();
				bundle.PutString ("video.path", videoPath);
				intent.PutExtras (bundle);
				StartActivityForResult (intent, PREVIEW_RESULT);
			};
		}

		void configureDurationGroup ()
		{
			durationGroup = (RadioGroup)this.FindViewById (Resource.Id.camcorderGroupTime);

			switch (userService.CurrentUser.RecordDuration) {
			case 1:
				durationGroup.Check(Resource.Id.camcorder1sec);
				break;
			case 3:
				durationGroup.Check (Resource.Id.camcorder3sec);
				break;
			case 5:
				durationGroup.Check (Resource.Id.camcorder5sec);
				break;
			}
		}

		int getDuration ()
		{
			switch (durationGroup.CheckedRadioButtonId) {
			case Resource.Id.camcorder1sec:
				return 1000;
			case Resource.Id.camcorder3sec:
				return 3000;
			case Resource.Id.camcorder5sec:
				return 5000;
			}

			throw new System.Exception ("cant reach");
		}

		void resetCountDown ()
		{
			countDownText.Text = "00:00";
		}

		void configureServicesAndParameters ()
		{
			mediaService = ((ISecondsApplication)this.Application).GetMediaService ();
			userService = ((ISecondsApplication)this.Application).GetUserService ();
			videoPath = this.Intent.Extras.GetString ("video.path");
		}
	}
}
