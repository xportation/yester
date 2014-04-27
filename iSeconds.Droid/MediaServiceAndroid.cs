using System.IO;
using Android.Content;
using Android.App;
using System;
using Android.Graphics;
using Java.IO;
using iSeconds.Domain;
using Xamarin.Media;
using Android.Media;
using Android.Provider;
using File = System.IO.File;
using Stream = System.IO.Stream;
using Android.OS;
using System.Collections.Generic;
using Android.Content.PM;

namespace iSeconds.Droid
{
	public class MediaServiceAndroid : IMediaService
	{
		private static readonly System.Object obj = new System.Object();
		private ActivityTracker activityTracker = null;
		private IRepository repository = null;
		private string mediaPath;
		private User user;
		private int cameraFPS= 15;
      //private bool usingNativeCamera = false;
		private Action<string> resultAction = null;

		public event EventHandler OnVideoRecorded;
		public event EventHandler OnThumbnailSaved;

		public MediaServiceAndroid(ActivityTracker activityTracker, IRepository repository, string mediaPath, User user)
		{
			this.activityTracker = activityTracker;
			this.repository = repository;
			this.mediaPath = mediaPath;
			this.user = user;
		}


		public void TakeVideo(DateTime date, Action<string> resultAction)
		{
         //if (usingNativeCamera)
				recordWithNativeCamera (date, resultAction);
         //else
         //	recordWithYesterCamera (date, resultAction);
		}

		public void SaveVideoThumbnail(string thumbnailPath, string videoPath)
		{
			AndroidMediaUtils.SaveVideoThumbnail (thumbnailPath, videoPath);

			if (OnThumbnailSaved != null)
				OnThumbnailSaved(this, EventArgs.Empty);
		}

		public void ConcatMovies(string compilationPath, DateTime startDate, DateTime endDate, int timelineId, bool onlyDefaultMovies)
		{
			Activity currentActivity = this.activityTracker.GetCurrentActivity();
			Intent mServiceIntent = new Intent(currentActivity, typeof(FFMpegService));

			// To use the ffmpeg service we simply create a intent object
			// and populate it's Extra`s with some properties (parameters).
			Bundle b = new Bundle();
			// Our message contains the following properties.
			// The ffmpeg command, here we will use the concat command (our service only implements this for now).
			b.PutString ("ffmpeg.command", "concat");

			// The output file.
			b.PutString ("ffmpeg.concat.output", compilationPath);

			IList<iSeconds.Domain.MediaInfo> videos = 
				repository.GetMediaInfoByPeriod (startDate, endDate, timelineId, onlyDefaultMovies);

			// A list with the file paths (absolute) of each video to be concatenated.
			IList<string> filesToConcat = new List<string>();

			// A list with a subtitle for each video.
			IList<string> subtitles = new List<string>();

			foreach (iSeconds.Domain.MediaInfo mediaInfo in videos) {
				filesToConcat.Add(mediaInfo.Path);
				subtitles.Add(generateSubtitleForMediaInfo(mediaInfo));
			}

			b.PutStringArrayList("ffmpeg.concat.filelist", filesToConcat);
			b.PutStringArrayList("ffmpeg.concat.subtitles", subtitles);

			mServiceIntent.PutExtras(b);

			currentActivity.StartService(mServiceIntent);
		}

		private string generateSubtitleForMediaInfo(iSeconds.Domain.MediaInfo mediaInfo)
		{
			DayInfo day = repository.GetDayInfo(mediaInfo.DayId);

			return String.Format("{0:d}", day.Date);
		}

		public void ShareVideo(string filename, string dialogTitle)
		{
			Activity currentActivity = this.activityTracker.GetCurrentActivity();

			Intent intent = new Intent (Intent.ActionSend);
			Java.IO.File filePath= new Java.IO.File(filename);
			intent.SetType ("video/mp4");
			intent.PutExtra(Intent.ExtraStream, Android.Net.Uri.FromFile(filePath));
			currentActivity.StartActivity(Intent.CreateChooser(intent, dialogTitle));
		}

		void recordWithNativeCamera (DateTime date, Action<string> resultAction)
		{
			lock (obj) {
				this.resultAction = resultAction;
				Activity currentActivity = this.activityTracker.GetCurrentActivity ();
				var picker = new MediaPicker (currentActivity);
				if (!picker.IsCameraAvailable || !picker.VideosSupported)
					return;
				picker.TakeVideoAsync (new StoreVideoOptions {
					Name = ISecondsUtils.StringifyDate ("movie", date),
					DesiredLength = System.TimeSpan.FromMilliseconds (user.RecordDuration * 1000 + 1000 / cameraFPS)
				}).ContinueWith (t =>  {
					if (t.IsCanceled)
						return;
					currentActivity.RunOnUiThread (() =>  {
						string path = t.Result.Path;
						string filename = System.IO.Path.GetFileName (path);
						string newPath = System.IO.Path.Combine (mediaPath, filename);
						System.IO.File.Copy (t.Result.Path, newPath, true);
						System.IO.File.Delete (t.Result.Path);

						CommitVideo(newPath);
					});
				});
			}
		}



		public void CommitVideo (string videoPath)
		{
			System.Diagnostics.Debug.Assert (resultAction != null);

			resultAction.Invoke (videoPath);
			if (OnVideoRecorded != null)
				OnVideoRecorded (this, EventArgs.Empty);
		}

		public void RevertVideo() {
			resultAction = null;
		}

		void recordWithYesterCamera (DateTime date, Action<string> _resultAction)
		{
			this.resultAction = _resultAction;

			Activity currentActivity = this.activityTracker.GetCurrentActivity ();
			Intent intent = new Intent (currentActivity, typeof(CamcorderActivity));

			string filename = ISecondsUtils.StringifyDate ("movie", date) + ".mp4";
			string path = System.IO.Path.Combine (mediaPath, filename);

			Bundle bundle = new Bundle ();
			bundle.PutString ("video.path", path);			
			intent.PutExtras (bundle);
			currentActivity.StartActivity (intent);
		}

	}
}
