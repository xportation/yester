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
using System.Threading;
using System.Globalization;

namespace iSeconds.Droid
{
	[Activity (Label = "CompileActivity", Theme = "@android:style/Theme.Dialog")]			
	public class CompileActivity : ISecondsActivity
	{
		private IRepository repository = null;
		private IPathService pathService = null;
		private IOptionsDialogService optionsDialogService = null;
		private User user = null;

		private DateTime startDate;
		private DateTime endDate;
		private int timelineId;
		private string compilationPath;
		private string thumbnailPath;

		private string timelineName;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			this.RequestWindowFeature(WindowFeatures.NoTitle);
			this.SetContentView (Resource.Layout.CompileView);

			Button okButton = this.FindViewById<Button> (Resource.Id.okButton);
			okButton.Click += (object sender, EventArgs e) => {

				addCompilation ();

				askToWait (concat);
			};

			Button cancelButton = this.FindViewById<Button> (Resource.Id.cancelButton);
			cancelButton.Click += (object sender, EventArgs e) => {
				this.Finish();
			};

			ISecondsApplication application = (ISecondsApplication)this.Application;
			repository = application.GetRepository ();
			pathService = application.GetPathService ();
			optionsDialogService = application.GetOptionsDialogService ();
			user = application.GetUserService ().CurrentUser;

			startDate = DateTime.FromBinary (Convert.ToInt64 (this.Intent.Extras.GetString ("ShareDate_Start")));
			endDate = DateTime.FromBinary (Convert.ToInt64 (this.Intent.Extras.GetString ("ShareDate_End")));
			timelineId = Convert.ToInt32 (this.Intent.Extras.GetString ("ShareDate_TimelineId"));


			string basePath = Path.Combine (
				pathService.GetCompilationPath (), ISecondsUtils.StringifyDate("compilation", DateTime.Now));
			compilationPath = thumbnailPath = basePath;
			compilationPath += ".mp4";
			thumbnailPath += ".png";

			timelineName = user.GetTimelineById (this.timelineId).Name;

			setDefaultTimelineName ();
			setDefaultTimelineDescription ();
		}



		void setDefaultTimelineName ()
		{
			string defaultName = timelineName + " (" + ISecondsUtils.DateToString(startDate, false) + " - " + ISecondsUtils.DateToString(endDate, false) + ")";
			EditText name = this.FindViewById<EditText>(Resource.Id.compilationNameEdit);
			name.Text = defaultName;
		}

		void setDefaultTimelineDescription ()
		{
			string defaultDescription = this.GetString(
				Resource.String.compilation_default_description, timelineName, ISecondsUtils.DateToString(startDate, false), ISecondsUtils.DateToString(endDate, false));
			EditText description = this.FindViewById<EditText>(Resource.Id.compilationDescriptionEdit);
			description.Text = defaultDescription;
		}

		void addCompilation ()
		{
			EditText name = this.FindViewById<EditText>(Resource.Id.compilationNameEdit);
			EditText description = this.FindViewById<EditText>(Resource.Id.compilationDescriptionEdit);

			Compilation compilation = new Compilation ();
			compilation.Name = name.Text;
			compilation.Description = description.Text;
			compilation.Begin = startDate;
			compilation.End = endDate;
			compilation.TimelineName = user.GetTimelineById (timelineId).Name;
			compilation.Path = compilationPath;
			compilation.ThumbnailPath = thumbnailPath;
			user.AddCompilation (compilation);
		}

		void askToWait (Action concat)
		{
			string msg = Resources.GetString (Resource.String.compilation_processing_message);
			optionsDialogService.ShowMessage (msg, concat);
		}

		void concat ()
		{
			Intent mServiceIntent = new Intent(this, typeof(FFMpegService));

			// To use the ffmpeg service we simply create a intent object
			// and populate it's Extra`s with some properties (parameters).
			Bundle b = new Bundle ();
			// Our message contains the following properties.
			// The ffmpeg command, here we will use the concat command (our service only implements this for now).
			b.PutString ("ffmpeg.command", "concat");

			// The output file.
			b.PutString ("ffmpeg.concat.output", compilationPath);

			// A list with the file paths (absolute) of each video to be concatenated.
			IList<MediaInfo> videos = repository.GetMediaInfoByPeriod (startDate, endDate, timelineId);
			IList<string> filesToConcat = new List<string> ();
			foreach (MediaInfo mediaInfo in videos) {
				filesToConcat.Add (mediaInfo.Path);
			}
			b.PutStringArrayList ("ffmpeg.concat.filelist", filesToConcat);

			mServiceIntent.PutExtras(b);

			StartService(mServiceIntent);
			this.Finish ();
		}
	}
}

