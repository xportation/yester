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
		private int selectedItem;
      private Activity activity;
      private IList<MediaInfo> videos;
      private Dictionary<int, View> viewItems;


      public VideoThumbnailsViewAdapter(Activity activity, IList<MediaInfo> videos)
		{
			selectedItem = -1;
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
				setBackgroundColor(position, view);
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
			selectedItem = position;
         foreach (KeyValuePair<int, View> viewItemPair in viewItems) {
            if (viewItemPair.Value == null)
               continue;

				setBackgroundColor(viewItemPair.Key, viewItemPair.Value);
         }
      }

		private void setBackgroundColor(int position, View view)
		{
			if (position == selectedItem)
				view.SetBackgroundColor(Color.Orange);
			else
				view.SetBackgroundColor(Color.White);
		}
	}

	interface VideoViewPreparer
	{
		void Prepare();
	}

	class ScreenUnlockReceiver : BroadcastReceiver
	{
		private VideoViewPreparer videoViewPreparer;

		public ScreenUnlockReceiver(VideoViewPreparer videoViewPreparer)
		{
			this.videoViewPreparer = videoViewPreparer;
		}

		public override void OnReceive(Context context, Android.Content.Intent intent)
		{
			videoViewPreparer.Prepare();
		}
	}

	[Activity (Label = "VideoPlayerActivity", ScreenOrientation = ScreenOrientation.Landscape, 
		ConfigurationChanges = ConfigChanges.KeyboardHidden|ConfigChanges.Orientation)]
	public class VideoPlayerActivity : ISecondsActivity, VideoViewPreparer
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

		private VideoView videoView;
		private ImageView playOverImage;

		private int lastVideoPosition = 0;
		private bool startingPaused = false;
		private bool alreadyPrepared = false;
		private ScreenUnlockReceiver screenUnlockReceiver = null;

		private const string CurrentVideoPaused= "currentVideoPaused";
		private const string CurrentVideoPlaying= "currentVideoPlaying";
		private const string CurrentVideoPosition= "currentVideoPosition";

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			this.RequestWindowFeature(WindowFeatures.NoTitle);
			this.SetContentView(Resource.Layout.VideoPlayer);

			ISecondsApplication application = (ISecondsApplication)this.Application;
			pathService = application.GetPathService();
			repository = application.GetRepository();

			videoView = FindViewById<VideoView>(Resource.Id.videoView);
         playOverImage= FindViewById<ImageView>(Resource.Id.imagePausePlay);
			date = FindViewById<TextView>(Resource.Id.textViewDate);
			TextViewUtil.ChangeForDefaultFont(date, this, 22f);

			configureActionBar(true, "");

			startDate= DateTime.FromBinary(Convert.ToInt64(this.Intent.Extras.GetString("ShareDate_Start")));
			endDate= DateTime.FromBinary(Convert.ToInt64(this.Intent.Extras.GetString("ShareDate_End")));
			timelineId= Convert.ToInt32(this.Intent.Extras.GetString("ShareDate_TimelineId"));

         videos = repository.GetMediaInfoByPeriod(startDate, endDate, timelineId);
			viewAdapter = new VideoThumbnailsViewAdapter(this, videos);

         thumbnails = FindViewById<ListView>(Resource.Id.videoThumbnails);
         thumbnails.Adapter = viewAdapter;
			thumbnails.Clickable = true;
         
         thumbnails.ItemClick += (sender, e) => 
            playVideo(e.Position);

			loadState(savedInstanceState);

			screenUnlockReceiver = new ScreenUnlockReceiver(this);
			IntentFilter filter = new IntentFilter(Intent.ActionUserPresent);
			RegisterReceiver(screenUnlockReceiver, filter);

			configureActions();
			Prepare();
		}

		protected override void OnDestroy()
		{
			UnregisterReceiver(screenUnlockReceiver);
			base.OnDestroy();
		}

		private void loadState(Bundle bundle)
		{
			if (bundle != null) {
				if (bundle.ContainsKey(CurrentVideoPlaying))
					currentVideo = bundle.GetInt(CurrentVideoPlaying);

				if (bundle.ContainsKey(CurrentVideoPaused))
					startingPaused = bundle.GetBoolean(CurrentVideoPaused);

				if (bundle.ContainsKey(CurrentVideoPosition))
					lastVideoPosition = bundle.GetInt(CurrentVideoPosition);
			}
		}

		private void configureActions()
		{
			startingPaused = false;
			alreadyPrepared = false;
			videoView.Completion+= (sender, e) => {
				playVideo(currentVideo+1);
			};

			videoView.Prepared += (sender, e) => {
				setDateLabel();
				videoView.Start();

				if (startingPaused) {
					RunOnUiThread (() => {
						videoView.SeekTo(lastVideoPosition);
						showPlayButton(true);
					});
					RunOnUiThread (() => {
						videoView.Pause();
					});
				}

				startingPaused= false;
			};

			videoView.Touch+= (object sender, View.TouchEventArgs e) => {
				if ((e.Event.Action & MotionEventActions.Mask) == MotionEventActions.Down) {
					if (videoView.IsPlaying)
						videoView.Pause();
					else
						videoView.Start();

					showPlayButton(!videoView.IsPlaying);
				}
			};
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
			startingPaused = !videoView.IsPlaying;
			outState.PutBoolean(CurrentVideoPaused, startingPaused);
			outState.PutInt(CurrentVideoPlaying, currentVideo);

			lastVideoPosition = videoView.CurrentPosition;
			outState.PutInt(CurrentVideoPosition, lastVideoPosition);
		}

		protected override void OnPause()
		{
			alreadyPrepared = false;
			videoView.StopPlayback();
			base.OnPause();
		}

      private void playVideo(int videoPosition)
      {
         if (videoPosition < videos.Count) {
				if (videoView.IsPlaying)
					videoView.StopPlayback();

            currentVideo = videoPosition;
            showPlayButton(false);
            viewAdapter.SelectViewItem(currentVideo);

            string filePath = videos[currentVideo].Path;
				videoView.SetVideoPath(filePath);
         } else {
            showPlayButton(true);
         }
      }

		private void setDateLabel()
		{
			if (currentVideo < videos.Count) {
				MediaInfo media = videos[currentVideo];
				DayInfo day = repository.GetDayInfo(media.DayId);

				DateTime dateTime = day.Date + media.TimeOfDay;
				date.Text= String.Format("{0:g}", dateTime);
			}
		}

		#region VideoViewPreparer implementation

		public void Prepare()
		{
			if (videos.Count > 0 && videoView != null && !alreadyPrepared) {
				alreadyPrepared = true;
				showPlayButton(false);
				viewAdapter.SelectViewItem(currentVideo);
				videoView.SetVideoPath(videos[currentVideo].Path);
			}
		}

		#endregion

		private void showPlayButton(bool mustShow)
		{
         if (mustShow)
            playOverImage.Visibility = ViewStates.Visible;
         else
            playOverImage.Visibility = ViewStates.Gone;
		}
	}
}

