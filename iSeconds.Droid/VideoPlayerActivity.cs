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
using Android.Graphics;
using Android.Content.Res;
using Android.Content.PM;

namespace iSeconds.Droid
{
	class VideoThumbnailsViewAdapter : BaseAdapter
	{
      private Activity activity;
      private IList<MediaInfo> videos;
      private Dictionary<int, View> viewItems;

      public VideoThumbnailsViewAdapter(Activity activity, IList<MediaInfo> videos)
		{
			this.videos = videos;
			this.activity = activity;

         viewItems = new Dictionary<int, View>();
         for (int index= 0; index < videos.Count; index++)
            viewItems.Add(index, null);
		}

		public override Java.Lang.Object GetItem(int position)
		{
			return null;
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View view = null;
         if (viewItems[position] != null)
            view = viewItems[position];
         else {
            MediaInfo media = videos[position];
            view = activity.LayoutInflater.Inflate(Resource.Layout.VideoThumbnail, null);
            ImageView imageView = view.FindViewById<ImageView>(Resource.Id.videoPlayerThumbnail);
            Bitmap thumbnail = BitmapFactory.DecodeFile(media.GetThumbnailPath());
            imageView.SetImageBitmap(thumbnail);
            view.SetBackgroundColor(Color.White);
            viewItems[position]= view;
         }

			return view;
		}

		public override int Count
		{
			get { return videos.Count; }
		}

      public void SelectViewItem(int position)
      {
         foreach (KeyValuePair<int, View> viewItemPair in viewItems) {
            if (viewItemPair.Value == null)
               continue;

            if (viewItemPair.Key == position)
               viewItemPair.Value.SetBackgroundColor(Color.Orange);
            else
               viewItemPair.Value.SetBackgroundColor(Color.White);
         }
      }
	}

   [Activity (Label = "VideoPlayerActivity", ScreenOrientation = ScreenOrientation.Landscape)]
	public class VideoPlayerActivity : ISecondsActivity, ISurfaceHolderCallback
	{
		private IPathService pathService = null;
		private IRepository repository = null;

		private DateTime startDate;
		private DateTime endDate;
		private int timelineId= 0;

      private int currentVideo = 0;
		private TextView date = null;
      private ListView thumbnails = null;
      private IList<MediaInfo> videos= null;
      private VideoThumbnailsViewAdapter viewAdapter = null;

      private Android.Media.MediaPlayer mediaPlayer;

		private ISurfaceHolder surfaceHolder;
		private SurfaceView surfaceView;
		private ImageView playOverImage;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate (bundle);

			this.RequestWindowFeature(WindowFeatures.NoTitle);
			this.SetContentView(Resource.Layout.VideoPlayer);

			ISecondsApplication application = (ISecondsApplication)this.Application;
			pathService = application.GetPathService();
			repository = application.GetRepository();

         playOverImage= FindViewById<ImageView>(Resource.Id.imagePausePlay);
			date = FindViewById<TextView>(Resource.Id.textViewDate);
			TextViewUtil.ChangeForDefaultFont(date, this, 22f);

			configureActionBar(true);

			startDate= DateTime.FromBinary(this.Intent.Extras.GetLong("ShareDate_Start"));
			endDate= DateTime.FromBinary(this.Intent.Extras.GetLong("ShareDate_End"));
			timelineId= this.Intent.Extras.GetInt("ShareDate_TimelineId");

         videos = repository.GetMediaInfoByPeriod(startDate, endDate, timelineId);
			viewAdapter = new VideoThumbnailsViewAdapter(this, videos);

         thumbnails = FindViewById<ListView>(Resource.Id.videoThumbnails);
         thumbnails.Adapter = viewAdapter;
			thumbnails.Clickable = true;
         
         thumbnails.ItemClick += (sender, e) => 
            playVideo(e.Position);
		}

      protected override void OnStart()
      {
         base.OnStart();

         surfaceView = (SurfaceView)FindViewById(Resource.Id.surfaceView);

         surfaceHolder = surfaceView.Holder;
         surfaceHolder.AddCallback(this);
         surfaceHolder.SetType(SurfaceType.PushBuffers);
      }

		protected override void OnStop()
		{
			mediaPlayer.Release();

			base.OnStop();
		}

      private void playVideo(int videoPosition)
      {
         if (videoPosition < videos.Count) {
            if (mediaPlayer.IsPlaying)
               mediaPlayer.Stop();

            currentVideo = videoPosition;
            showPlayButton(false);
            viewAdapter.SelectViewItem(currentVideo);

            string filePath = videos[currentVideo].Path;

            mediaPlayer.Reset();
            mediaPlayer.SetDataSource(filePath);
            mediaPlayer.PrepareAsync();
         } else {
            showPlayButton(true);
         }
      }

		#region ISurfaceHolderCallback implementation

		public void SurfaceChanged(ISurfaceHolder holder, Format format, int width, int height)
		{
		}

		public void SurfaceCreated(ISurfaceHolder holder)
		{
         mediaPlayer = new Android.Media.MediaPlayer();
         mediaPlayer.SetWakeMode(this, WakeLockFlags.Full);
         mediaPlayer.SetDisplay(surfaceHolder);

         mediaPlayer.Completion += (sender, e) => {
            playVideo(currentVideo + 1);
         };

         mediaPlayer.Prepared += (sender, e) => {
				setDateLabel();
            mediaPlayer.Start();
         };

			prepareSurface();
		}

		public void SurfaceDestroyed(ISurfaceHolder holder)
		{
			mediaPlayer.Release();
		}

		#endregion

		private void setDateLabel()
		{
			if (currentVideo < videos.Count) {
				MediaInfo media = videos[currentVideo];
				DayInfo day = repository.GetDayInfo(media.DayId);

				date.Text= String.Format("{0:g}", day.Date);
			}
		}

		private void prepareSurface()
		{
			if (videos.Count > 0) {
				currentVideo = 0;
            showPlayButton(false);
            viewAdapter.SelectViewItem(currentVideo);
				mediaPlayer.SetDataSource(videos[currentVideo].Path);
				mediaPlayer.Prepare();

				int surfaceView_Width = surfaceView.Width;
				int surfaceView_Height = surfaceView.Height;

				float video_Width = mediaPlayer.VideoWidth;
				float video_Height = mediaPlayer.VideoHeight;

				float ratio_width = surfaceView_Width/video_Width;
				float ratio_height = surfaceView_Height/video_Height;
				float aspectratio = video_Width/video_Height;

				var layoutParams = surfaceView.LayoutParameters;

				if (ratio_width > ratio_height){
					layoutParams.Width = (int) (surfaceView_Height * aspectratio);
					layoutParams.Height = surfaceView_Height;
				}else{
					layoutParams.Width = surfaceView_Width;
					layoutParams.Height = (int) (surfaceView_Width / aspectratio);
				}

				surfaceView.LayoutParameters= layoutParams;

				surfaceView.Click += (sender, e) => {
					if (mediaPlayer.IsPlaying)
						mediaPlayer.Pause();
					else
						mediaPlayer.Start();

					showPlayButton(!mediaPlayer.IsPlaying);
				};
			}
		}

		private void showPlayButton(bool mustShow)
		{
         if (mustShow)
            playOverImage.Visibility = ViewStates.Visible;
         else
            playOverImage.Visibility = ViewStates.Gone;
		}
	}
}

