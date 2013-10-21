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
using Android.Util;

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
			pathService = application.GetPathService();

			repository = application.GetRepository();

         if (this.Intent.Extras.ContainsKey("TimelineId"))
			   timelineId = Convert.ToInt32(this.Intent.Extras.GetString("TimelineId"));

			configureActionBar(true);
			configureTextViewFonts();
			connectToShareButton();
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

		private void connectToShareButton ()
		{
			Button shareButton = this.FindViewById<Button> (Resource.Id.shareButton);
			shareButton.Click += (sender, e) => {
				DatePicker start = this.FindViewById<DatePicker> (Resource.Id.dateStartPeriod);
				DatePicker end = this.FindViewById<DatePicker> (Resource.Id.dateEndPeriod);

				DateTime startDate = new DateTime (start.Year, start.Month + 1, start.DayOfMonth);
				DateTime endDate = new DateTime (end.Year, end.Month + 1, end.DayOfMonth);
								
				Intent intent= new Intent(this, typeof(VideoPlayerActivity));
				intent.PutExtra("ShareDate_Start", startDate.ToBinary());
				intent.PutExtra("ShareDate_End", endDate.ToBinary());
				intent.PutExtra("ShareDate_TimelineId", timelineId);
				this.StartActivity(intent);
			};
		}

      private void setShareButtonEnabled(bool enabled)
      {
         Button shareButton = this.FindViewById<Button> (Resource.Id.shareButton);
         shareButton.Enabled = enabled;
      }

		void share (string result)
		{
			Intent intent = new Intent (Intent.ActionSend);
         Java.IO.File filePath= new Java.IO.File(result);
         intent.SetType ("video/mp4");
         intent.PutExtra(Intent.ExtraStream, Android.Net.Uri.FromFile(filePath));
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