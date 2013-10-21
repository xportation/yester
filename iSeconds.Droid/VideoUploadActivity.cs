using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using iSeconds.Domain;
using Android.Graphics;
using System.Collections.Generic;

namespace iSeconds.Droid
{
   //TODO [leonardo] o dayoptions tem algo parecido
   class VideoUploadListAdapter : BaseAdapter
   {
      private Activity context = null;
      private IList<MediaInfo> videosToUpload;

      public VideoUploadListAdapter (Activity context, IList<MediaInfo> videosToUpload)
      {
         this.context = context;
         this.videosToUpload = videosToUpload;
      }

      public override Java.Lang.Object GetItem (int position)
      {
         return null;
      }

      public override long GetItemId (int position)
      {
         return position;
      }

      public override View GetView (int position, View convertView, ViewGroup parent)
      {
         View view = convertView;
         TextView videoName = null;
         MediaInfo media = videosToUpload[position];

         if (view == null) 
         {
            view = context.LayoutInflater.Inflate(Resource.Layout.VideoUploadItem, null);

            videoName = view.FindViewById<TextView>(Resource.Id.videoUploadName);
            TextViewUtil.ChangeForDefaultFont(videoName, context, 20f);
         }

         videoName = view.FindViewById<TextView>(Resource.Id.videoUploadName);

         videoName.Text = media.Path;

         ImageView imageView = view.FindViewById<ImageView> (Resource.Id.videoUploadThumbnail);
         Bitmap thumbnail = BitmapFactory.DecodeFile(media.GetThumbnailPath());
         imageView.SetImageBitmap(thumbnail);

         return view;
      }

      public override int Count {
         get { return videosToUpload.Count; }
      }
   }

	[Activity (Label = "VideoUploadActivity", Icon="@drawable/Icon")]
   public class VideoUploadActivity : ISecondsActivity
   {
      Intent videoUploadServiceIntent;
      VideoUploadServiceConnection videoUploadServiceConnection;

      private ListView listView = null;
		private VideoUploadListAdapter adapter = null;

      private IPathService pathService = null;
      private IRepository repository = null;

		private DateTime startDate;
		private DateTime endDate;
		private int timelineId= 0;

      protected override void OnCreate(Bundle bundle)
      {
         base.OnCreate (bundle);

			this.RequestWindowFeature(WindowFeatures.NoTitle);
			this.SetContentView(Resource.Layout.VideoUpload);

         ISecondsApplication application = (ISecondsApplication)this.Application;
         pathService = application.GetPathService();
         repository = application.GetRepository();

			startDate= DateTime.FromBinary(this.Intent.Extras.GetLong("ShareDate_Start"));
			endDate= DateTime.FromBinary(this.Intent.Extras.GetLong("ShareDate_End"));
			timelineId= this.Intent.Extras.GetInt("ShareDate_TimelineId");

			adapter = new VideoUploadListAdapter(this, repository.GetMediaInfoByPeriod(startDate, endDate, timelineId));
         listView = this.FindViewById<ListView>(Resource.Id.videosList);
         listView.Adapter = adapter;

         startVideoUploadService();
			connectToService();
      }

      private void startVideoUploadService()
      {
         videoUploadServiceIntent = new Intent(this, typeof(VideoUploadService));
         StartService(videoUploadServiceIntent);
      }

      private void connectToService()
      {
         videoUploadServiceConnection = new VideoUploadServiceConnection();
         BindService(videoUploadServiceIntent, videoUploadServiceConnection, Bind.AutoCreate);

         videoUploadServiceConnection.onServiceConnected+= () => {
            var binder= videoUploadServiceConnection.Binder;
            if (binder != null) {
               string pathResult = pathService.GetMediaPath() + "/meus_videos.mp4";
               binder.GetVideoUploadService().Share(repository.GetVideosFromRange(startDate, endDate, timelineId), pathResult);
            }
         };
      }

      protected override void OnStop ()
      {
         base.OnStop ();

         UnbindService(videoUploadServiceConnection);
      }
   }
}

