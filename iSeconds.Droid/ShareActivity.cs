using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content;
using Android.Net;

namespace iSeconds.Droid
{
	[Activity(Label = "Share")]
	public class ShareActivity : ISecondsActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			this.RequestWindowFeature(WindowFeatures.NoTitle);
			this.SetContentView(Resource.Layout.ShareView);

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
				Intent intent= new Intent(Intent.ActionSend);
				Uri videoUri= Uri.Parse("file:///mnt/sdcard/iSeconds/Videos/movie_24_5_2013_22_44_16.mp4");
//				Uri videoUri= Uri.Parse("file:///mnt/sdcard/iSeconds/test_app.jpg");
				intent.SetType("video/mp4");
//				intent.SetType("image/jpg");
				intent.PutExtra(Intent.ExtraStream, videoUri);
				StartActivity(Intent.CreateChooser(intent,"Share your timeline to..."));
			};
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