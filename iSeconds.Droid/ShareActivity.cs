using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content;
using Android.Net;
using RestSharp;
using System;

//using System.IO;
using iSeconds.Domain;
using System.IO;
using System.Collections.Generic;

namespace iSeconds.Droid
{
	[Activity(Label = "Share")]
	public class ShareActivity : ISecondsActivity
	{
		private IPathService pathService = null;
		private IRepository repository = null;
		private int timelineId = -1;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			this.RequestWindowFeature (WindowFeatures.NoTitle);
			this.SetContentView (Resource.Layout.ShareView);

			ISecondsApplication application = (ISecondsApplication)this.Application;
			pathService = application.GetPathService ();

			repository = application.GetRepository ();

			timelineId = Convert.ToInt32 (this.Intent.Extras.GetString ("TimelineId"));

			configureActionBar (true);
			configureTextViewFonts ();
			connectToShareButton ();
		}

		private void configureTextViewFonts ()
		{
			TextView startPeriod = this.FindViewById<TextView> (Resource.Id.textStartPeriod);
			TextViewUtil.ChangeForDefaultFont (startPeriod, this, 22f);

			TextView endPeriod = this.FindViewById<TextView> (Resource.Id.textEndPeriod);
			TextViewUtil.ChangeForDefaultFont (endPeriod, this, 22f);

			Button shareButton = this.FindViewById<Button> (Resource.Id.shareButton);
			TextViewUtil.ChangeForDefaultFont (shareButton, this, 22f);
		}

		void connectToShareButton ()
		{
			Button shareButton = this.FindViewById<Button> (Resource.Id.shareButton);
			shareButton.Click += (sender, e) => {

				DatePicker start = this.FindViewById<DatePicker> (Resource.Id.dateStartPeriod);
				DatePicker end = this.FindViewById<DatePicker> (Resource.Id.dateEndPeriod);

				DateTime startDate = new DateTime (start.Year, start.Month + 1, start.DayOfMonth);
				DateTime endDate = new DateTime (end.Year, end.Month + 1, end.DayOfMonth);

				string result = concat (startDate, endDate);
				share (result);
			};
		}

		string concat (DateTime start, DateTime end)
		{
			var client = new RestClient ("http://thawing-lowlands-7118.herokuapp.com");
			var request = new RestRequest ("begin", Method.GET);

			IRestResponse response = client.Execute (request);
			var sessionKey = response.Content; // raw content as string
			Console.WriteLine (sessionKey);

			IList<string> videoPaths = repository.GetVideosFromRange (start, end, timelineId);

			foreach (string videoPath in videoPaths) {  
				Console.WriteLine (uploadFile (client, videoPath, sessionKey).Content);
			}
			var result = pathService.GetMediaPath () + "/newresult.mp4";

			RestRequest endRequest = new RestRequest ("end", Method.POST);
			endRequest.AddParameter ("sessionKey", sessionKey);

			byte[] rawbytes = client.DownloadData (endRequest);
			Console.WriteLine (rawbytes);
			Console.WriteLine (rawbytes.Length);

			System.IO.FileStream _FileStream = 
            new System.IO.FileStream (result, System.IO.FileMode.Create,
			                                  System.IO.FileAccess.Write);
			// Writes a block of bytes to this stream using data from
			// a byte array.
			_FileStream.Write (rawbytes, 0, rawbytes.Length);

			// close file stream
			_FileStream.Close ();

			return result;
		}

		private IRestResponse uploadFile (RestClient client, string path, string sessionKey)
		{
			RestRequest upload = new RestRequest ("upload", Method.POST);
			upload.AddParameter ("sessionKey", sessionKey);
			upload.AddFile ("videos", path);
			return client.Execute (upload);
		}

		void share (string result)
		{
			Intent intent = new Intent (Intent.ActionSend);
			Android.Net.Uri videoUri = Android.Net.Uri.Parse (result);
			intent.SetType ("video/mp4");
			intent.PutExtra (Intent.ExtraStream, videoUri);
			StartActivity (Intent.CreateChooser (intent, "Share your timeline to..."));
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.actionbar_back_to_home:
				OnSearchRequested ();
					//viewModel.BackToHomeCommand.Execute(null);
				this.Finish ();
				return true;
			}

			return base.OnOptionsItemSelected (item);
		}
	}
}