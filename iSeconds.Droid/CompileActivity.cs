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
using System.IO;

namespace iSeconds.Droid
{
	[Activity (Label = "CompileActivity", Theme = "@android:style/Theme.Dialog")]			
	public class CompileActivity : Activity
	{
		private Messenger serviceMessenger = null;
		private IServiceConnection serviceConnection = null;
		private IRepository repository = null;
		private IPathService pathService = null;
		private User user = null;

		private DateTime startDate;
		private DateTime endDate;
		private int timelineId;
		private string compilationPath;
		private string thumbnailPath;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			this.SetContentView (Resource.Layout.CompileView);

			Button okButton = this.FindViewById<Button> (Resource.Id.okButton);
			okButton.Click += (object sender, EventArgs e) => {
				EditText name = this.FindViewById<EditText>(Resource.Id.compilationNameEdit);
				EditText description = this.FindViewById<EditText>(Resource.Id.compilationDescriptionEdit);

				Toast.MakeText(this, "right", ToastLength.Short).Show();

				// TODO: fazer o bind logo no inicio da aplicaçao

				Compilation compilation = new Compilation();
				compilation.Name = name.Text;
				compilation.Description = description.Text;
				compilation.Begin = startDate;
				compilation.End = endDate;
				compilation.TimelineName = user.GetTimelineById(timelineId).Name;
				compilation.Path = compilationPath;
				compilation.ThumbnailPath = thumbnailPath;

				user.AddCompilation(compilation);

				bindService ();

				//this.Finish();
			};

			Button cancelButton = this.FindViewById<Button> (Resource.Id.cancelButton);
			cancelButton.Click += (object sender, EventArgs e) => {
				this.Finish();
			};

			ISecondsApplication application = (ISecondsApplication)this.Application;
			repository = application.GetRepository ();
			pathService = application.GetPathService ();
			user = application.GetUserService ().CurrentUser;

			startDate = DateTime.FromBinary (Convert.ToInt64 (this.Intent.Extras.GetString ("ShareDate_Start")));
			endDate = DateTime.FromBinary (Convert.ToInt64 (this.Intent.Extras.GetString ("ShareDate_End")));
			timelineId = Convert.ToInt32 (this.Intent.Extras.GetString ("ShareDate_TimelineId"));


			string basePath = Path.Combine (
				pathService.GetCompilationPath (), ISecondsUtils.StringifyDate("compilation", DateTime.Now));
			compilationPath = thumbnailPath = basePath;
			compilationPath += ".mp4";
			thumbnailPath += ".png";
		}

		protected override void OnStart ()
		{
			base.OnStart ();
		}

		public void Concat ()
		{
			// To use the ffmpeg service we simply create a Message object
			// and populate it's Data field with some properties (parameters).
			Message message = Message.Obtain ();
			Bundle b = new Bundle ();
			// Our message contains the following properties.
			// The ffmpeg command, here we will use the concat command (our service only implements this for now).
			b.PutString ("ffmpeg.command", "concat");
			// The output file.

			b.PutString ("ffmpeg.concat.output", compilationPath);

			IList<MediaInfo> videos = repository.GetMediaInfoByPeriod (startDate, endDate, timelineId);

			// A list with the file paths (absolute) of each video to be concatenated.
			IList<string> filesToConcat = new List<string> ();
			foreach (MediaInfo mediaInfo in videos) {
				filesToConcat.Add (mediaInfo.Path);
			}
			b.PutStringArrayList ("ffmpeg.concat.filelist", filesToConcat);
			message.Data = b;

			// Send the message to our service and let it do it's job :)
			serviceMessenger.Send (message);

			//Toast.MakeText (this, "when the compilation was finished you will be notified", ToastLength.Long).Show ();
			this.Finish ();
		}

		void bindService ()
		{
			// We just need this string at our Intent, except for that, no direct dependency on the service.
			var ffmpegServiceIntent = new Intent ("com.broditech.iseconds.FFMpegService");
			serviceConnection = new FFMpegServiceConnection (this);
			BindService (ffmpegServiceIntent, serviceConnection, Bind.AutoCreate);
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

