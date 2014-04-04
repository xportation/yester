using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Views;
using Android.Util;
using Android.Widget;

namespace iSeconds.Droid
{
   [Activity (Label = "Yester Camcorder", Theme = "@android:style/Theme.NoTitleBar.Fullscreen", 
      ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation)]
   public class CamcorderActivity : Activity
   {
      bool isRecording = false;
      CamcorderView camcorderView = null;

      protected override void OnCreate (Bundle savedInstanceState)
      {
         base.OnCreate (savedInstanceState);

         SetContentView (Resource.Layout.Camcorder);

         var layout = FindViewById<RelativeLayout> (Resource.Id.layoutCamcorder);
         camcorderView = new CamcorderView (this, 0);
         ViewGroup.LayoutParams camcorderLayoutParams = new ViewGroup.LayoutParams (ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
         layout.AddView (camcorderView, 0, camcorderLayoutParams);

         var recordButton = FindViewById<ImageButton> (Resource.Id.camcorderRecordButton);
         recordButton.Click += (sender, e) => {
            if (!isRecording) {

               string filename = System.IO.Path.Combine (Android.OS.Environment.ExternalStorageDirectory.Path, "Camcoder");

               if (!System.IO.Directory.Exists (filename)) {
                  System.IO.Directory.CreateDirectory (filename);
               }

               filename = System.IO.Path.Combine (filename, "video.mp4");

               int duration = getDuration ();
               camcorderView.StartRecording (filename, duration);
            } else {
               camcorderView.StopRecording ();
               StartActivity (typeof(CamcorderPreview));
            }

            isRecording = !isRecording;
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
