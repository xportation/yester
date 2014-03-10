using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Views;
using Android.Util;
using Android.Widget;

namespace Yester_Camcorder
{
	[Activity (Label = "Yester Camcorder", Theme = "@android:style/Theme.NoTitleBar.Fullscreen", 
      ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation, MainLauncher = true)]
   public class Camcorder : Activity
	{
      bool isRecording = false;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView(Resource.Layout.Camcorder);

			var layout = FindViewById<RelativeLayout>(Resource.Id.layoutCamcorder);
			var camcorderView = new CamcorderView(this, 0);
			ViewGroup.LayoutParams camcorderLayoutParams = new ViewGroup.LayoutParams (ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
			layout.AddView(camcorderView, 0, camcorderLayoutParams);

            var recordButton = FindViewById<ImageButton> (Resource.Id.camcorderRecordButton);
            recordButton.Click += (sender, e) => {
            if (!isRecording)
               camcorderView.StartRecording("/sdcard/caralho.mp4");
            else {
               camcorderView.StopRecording();
               StartActivity(typeof(CamcorderPreview));
            }

            isRecording= !isRecording;
         };

         ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem);
         adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
         foreach (Camera.Size size in camcorderView.CameraSizes)
            adapter.Add (size.Width.ToString () + " x " + size.Height.ToString ());

         var spinner = FindViewById<Spinner> (Resource.Id.camcorderResolution);
         spinner.Adapter = adapter;
         spinner.ItemSelected += (sender, e) => {
            Android.Graphics.Rect rect= new Android.Graphics.Rect();
            layout.GetDrawingRect(rect);
            camcorderView.SetPreviewSize(e.Position, rect.Width(), rect.Height());
         };
		}
	}
}
