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

namespace iSeconds.Droid
{
   [Activity (Label = "CamcorderPreview", Theme = "@android:style/Theme.NoTitleBar.Fullscreen")]         
	public class CamcorderPreview : Activity//ISecondsActivity
   {
		public const Result RetakeResult = Result.FirstUser + 1; 

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			string videoPath = this.Intent.Extras.GetString ("video.path");

			SetContentView(Resource.Layout.CamcorderPreview);
			var videoView = FindViewById<VideoView> (Resource.Id.camcorderPreviewVideo);
			videoView.SetVideoPath (videoPath);
			videoView.Start ();

			Button confirmButton = (Button)this.FindViewById (Resource.Id.camcorderPreviewConfirmButton);
			confirmButton.Click += (object sender, EventArgs e) => {

				this.SetResult(Result.Ok);
				this.Finish();

			};

			Button retakeButton = (Button)this.FindViewById (Resource.Id.camcorderPreviewRetakeButton);
			retakeButton.Click += (object sender, EventArgs e) => {

				this.SetResult(RetakeResult);
				this.Finish();
			};

			Button cancelButton = (Button)this.FindViewById (Resource.Id.camcorderPreviewCancelButton);
			cancelButton.Click += (object sender, EventArgs e) => {

				this.SetResult(Result.Canceled);
				this.Finish();
			};

		}
	}
}

