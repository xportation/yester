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

namespace Yester_Camcorder
{
   [Activity (Label = "CamcorderPreview", Theme = "@android:style/Theme.NoTitleBar.Fullscreen")]         
   public class CamcorderPreview : Activity
   {
      protected override void OnCreate (Bundle bundle)
      {
         base.OnCreate (bundle);

         SetContentView(Resource.Layout.CamcorderPreview);
         var videoView = FindViewById<VideoView> (Resource.Id.camcorderPreviewVideo);
         videoView.SetVideoPath ("/sdcard/Camcoder/video.mp4");
         videoView.Start ();
      }
   }
}

