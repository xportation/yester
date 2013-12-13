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
	[Activity (Label = "CompileActivity")]			
	public class CompileActivity : Activity
	{
		private Messenger serviceMessenger;
		private IServiceConnection serviceConnection;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			this.SetContentView (Resource.Layout.CompileView);

			//bindService ();


		}

		public void Concat ()
		{
			DateTime startDate = DateTime.FromBinary (Convert.ToInt64 (this.Intent.Extras.GetString ("ShareDate_Start")));
			DateTime endDate = DateTime.FromBinary (Convert.ToInt64 (this.Intent.Extras.GetString ("ShareDate_End")));
			int timelineId = Convert.ToInt32 (this.Intent.Extras.GetString ("ShareDate_TimelineId"));
			ISecondsApplication application = (ISecondsApplication)this.Application;
			IRepository repository = application.GetRepository ();
			IList<MediaInfo> videos = repository.GetMediaInfoByPeriod (startDate, endDate, timelineId);
			// To use the ffmpeg service we simply create a Message object
			// and populate it's Data field with some properties (parameters).
			Message message = Message.Obtain ();
			Bundle b = new Bundle ();
			// Our message contains the following properties.
			// The ffmpeg command, here we will use the concat command (our service only implements this for now).
			b.PutString ("ffmpeg.command", "concat");
			// The output file.
			b.PutString ("ffmpeg.concat.output", "/sdcard/v3.mp4");
			IList<string> filesToConcat = new List<string> ();
			foreach (MediaInfo mediaInfo in videos) {
				filesToConcat.Add (mediaInfo.Path);
			}
			/*filesToConcat.Add("/mnt/sdcard/v1.mp4");
			filesToConcat.Add("/mnt/sdcard/v2.mp4");*/// A list with the file paths (absolute) of each video to be concatenated.
			b.PutStringArrayList ("ffmpeg.concat.filelist", filesToConcat);
			message.Data = b;
			// Send the message to our service and let it do it's job :)
			serviceMessenger.Send (message);
		}

		void bindService ()
		{
			// We just need this string at our Intent, except for that, no direct dependency on the service.
			var ffmpegServiceIntent = new Intent ("com.broditech.iseconds.FFMpegService");
			serviceConnection = new FFMpegServiceConnection (this);
			BindService (ffmpegServiceIntent, serviceConnection, Bind.AutoCreate);
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			bindService ();

		}


		class FFMpegServiceConnection : Java.Lang.Object, IServiceConnection
		{
			CompileActivity activity;

			public FFMpegServiceConnection (CompileActivity activity) {
				this.activity = activity;
			}

			public void OnServiceConnected (ComponentName name, IBinder service) {
				activity.serviceMessenger = new Messenger (service);
				activity.Concat ();
			}

			public void OnServiceDisconnected (ComponentName name) {
				activity.serviceMessenger.Dispose();
				activity.serviceMessenger = null;
			}
		}
	}
}

