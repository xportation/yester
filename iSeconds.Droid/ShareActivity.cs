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

namespace iSeconds.Droid
{
	[Activity(Label = "Share")]
	public class ShareActivity : ISecondsActivity
	{
      private IPathService pathService = null;
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			this.RequestWindowFeature(WindowFeatures.NoTitle);
			this.SetContentView(Resource.Layout.ShareView);

         ISecondsApplication application = (ISecondsApplication) this.Application;
         pathService = application.GetPathService ();

			configureActionBar(true);
			configureTextViewFonts();
			connectToShareButton();
		}

		private void configureTextViewFonts()
		{
			TextView startPeriod = this.FindViewById<TextView>(Resource.Id.textStartPeriod);
			TextViewUtil.ChangeForDefaultFont(startPeriod, this, 22f);

			TextView endPeriod = this.FindViewById<TextView>(Resource.Id.textEndPeriod);
			TextViewUtil.ChangeForDefaultFont(endPeriod, this, 22f);

			Button shareButton = this.FindViewById<Button>(Resource.Id.shareButton);
			TextViewUtil.ChangeForDefaultFont(shareButton, this, 22f);
		}

		void connectToShareButton()
		{
			Button shareButton = this.FindViewById<Button>(Resource.Id.shareButton);
			shareButton.Click+= (sender, e) => {
            concat ();
			/*	Intent intent= new Intent(Intent.ActionSend);
				Uri videoUri= Uri.Parse("file:///mnt/sdcard/iSeconds/Videos/movie_24_5_2013_22_44_16.mp4");
//				Uri videoUri= Uri.Parse("file:///mnt/sdcard/iSeconds/test_app.jpg");
				intent.SetType("video/mp4");
//				intent.SetType("image/jpg");
				intent.PutExtra(Intent.ExtraStream, videoUri);
				StartActivity(Intent.CreateChooser(intent,"Share your timeline to..."));*/
			};
		}

      void concat()
      {
         /*var client = new RestClient ("http://rxnav.nlm.nih.gov/REST/RxTerms/rxcui/");

         var request = new RestRequest (String.Format ("{0}/allinfo", "198440"));
         client.ExecuteAsync (request, response => {
            Console.WriteLine (response.Content);
         });*/

         var client = new RestClient("http://thawing-lowlands-7118.herokuapp.com");
         var request = new RestRequest("begin", Method.GET);

         IRestResponse response = client.Execute(request);
         var sessionKey = response.Content; // raw content as string
         Console.WriteLine (sessionKey);

         var path1 = pathService.GetMediaPath () + "/movie_17_5_2013_13_07_54.mp4";
         var path2 = pathService.GetMediaPath() +  "/movie_28_4_2013_16_38_16.mp4";
         var result = pathService.GetMediaPath() +  "/newresult.mp4";

         /*if (!System.IO.FileExist (path1))
            throw new Exception ("file dont exist");

         if (!FileExist (path2))
            throw new Exception ("file dont exist");*/

         Console.WriteLine(uploadFile(client, path1, sessionKey).Content);
         Console.WriteLine(uploadFile(client, path2, sessionKey).Content);

         RestRequest endRequest = new RestRequest("end", Method.POST);
         endRequest.AddParameter("sessionKey", sessionKey);

         byte[] rawbytes = client.DownloadData (endRequest);
         Console.WriteLine (rawbytes);
         Console.WriteLine (rawbytes.Length);

         System.IO.FileStream _FileStream = 
            new System.IO.FileStream(result, System.IO.FileMode.Create,
                                     System.IO.FileAccess.Write);
         // Writes a block of bytes to this stream using data from
         // a byte array.
         _FileStream.Write(rawbytes, 0, rawbytes.Length);

         // close file stream
         _FileStream.Close();

         Intent intent= new Intent(Intent.ActionSend);
         Android.Net.Uri videoUri= Android.Net.Uri.Parse(result);
//          Uri videoUri= Uri.Parse("file:///mnt/sdcard/iSeconds/test_app.jpg");
         intent.SetType("video/mp4");
//          intent.SetType("image/jpg");
         intent.PutExtra(Intent.ExtraStream, videoUri);
         StartActivity(Intent.CreateChooser(intent,"Share your timeline to..."));
      }

      private IRestResponse uploadFile(RestClient client, string path, string sessionKey)
      {
         RestRequest upload = new RestRequest("upload", Method.POST);
         upload.AddParameter("sessionKey", sessionKey);
         upload.AddFile("videos", path);
         return client.Execute(upload);
      }

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.actionbar_back_to_home:
					OnSearchRequested();
					//viewModel.BackToHomeCommand.Execute(null);
					this.Finish();
					return true;
			}

			return base.OnOptionsItemSelected(item);
		}
	}
}