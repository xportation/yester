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

		const int PREVIEW_RESULT = 1;


		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			mediaService = ((ISecondsApplication)this.Application).GetMediaService ();

			videoPath = this.Intent.Extras.GetString ("video.path");

			SetContentView (Resource.Layout.Camcorder);

			var layout = FindViewById<RelativeLayout> (Resource.Id.layoutCamcorder);
			camcorderView = new CamcorderView (this, 0);
			ViewGroup.LayoutParams camcorderLayoutParams = new ViewGroup.LayoutParams (ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
			layout.AddView (camcorderView, 0, camcorderLayoutParams);

			var recordButton = FindViewById<ImageButton> (Resource.Id.camcorderRecordButton);
			recordButton.Click += (sender, e) => {
				if (!isRecording) {              

					isRecording = true;

					int duration = getDuration ();
					camcorderView.StartRecording (videoPath, duration);
				} else {

					// TODO: implement pause

					//camcorderView.StopRecording ();
					//StartActivity (typeof(CamcorderPreview));
				}            
			};

			ArrayAdapter<string> adapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleSpinnerItem);
			adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);

			var spinner = FindViewById<Spinner> (Resource.Id.camcorderResolution);
			spinner.Adapter = adapter;
			spinner.ItemSelected += (s, evt) => {
				Android.Graphics.Rect rect = new Android.Graphics.Rect ();
				layout.GetDrawingRect (rect); 
				camcorderView.SetPreviewSize (evt.Position, rect.Width (), rect.Height ());
			};

			camcorderView.OnCameraReady += (object sender, EventArgs e) => {

				adapter.Clear();

				foreach (Camera.Size size in camcorderView.CameraSizes)
					adapter.Add (size.Width.ToString () + " x " + size.Height.ToString ());
			};

			camcorderView.OnVideoRecorded += (object sender, EventArgs e) => {

				isRecording = false;

				Intent intent = new Intent(this, typeof(CamcorderPreview));
				Bundle bundle = new Bundle();
				bundle.PutString("video.path", videoPath);
				intent.PutExtras(bundle);

				//intent.SetFlags(intent.Flags | ActivityFlags.NoHistory);
				StartActivityForResult(intent, PREVIEW_RESULT);
			};
		}     

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data) {

			if (requestCode == PREVIEW_RESULT) {

				switch (resultCode) {
				case Result.Ok: 
					mediaService.CommitVideo (videoPath);
					this.Finish ();
					break;

				case Result.Canceled:
					mediaService.RevertVideo ();
					this.Finish ();
					break;

				case CamcorderPreview.RetakeResult:
					// nothing to do for now... let activity restart
					break;				
				
				}
			}
		}

		int getDuration ()
		{
			RadioGroup group = (RadioGroup)this.FindViewById (Resource.Id.camcorderGroupTime);
			switch (group.CheckedRadioButtonId) {
			case Resource.Id.camcorder1sec:
				return 1000;
			case Resource.Id.camcorder3sec:
				return 3000;
			case Resource.Id.camcorder5sec:
				return 5000;
			}

			throw new Exception ("cant reach");
		}
	}
}
