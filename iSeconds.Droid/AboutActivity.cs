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
	[Activity (Label = "AboutActivity")]			
	public class AboutActivity : ISecondsActivity
	{
		private const int TutorialDialogId = 20;
		private IOptionsDialogService dialogService = null;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			this.RequestWindowFeature(WindowFeatures.NoTitle);
			this.SetContentView(Resource.Layout.AboutView);

			ISecondsApplication application = (ISecondsApplication)this.Application;
			dialogService = application.GetOptionsDialogService();

			configureActionBar(true, "");
			configureHyperlinks();

			#if !YESTER_LITE
			LinearLayout liteLayout= FindViewById<LinearLayout>(Resource.Id.layout_about_yester_lite);
			liteLayout.Visibility= ViewStates.Gone;
			#endif
			setupAds();
		}

		void configureHyperlinks()
		{
			ImageView facebookImageView = FindViewById<ImageView>(Resource.Id.imageFacebook);
			facebookImageView.Click+= (sender, e) => {
				Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://www.facebook.com/yester.mobile"));
				StartActivity(intent);
			};

			ImageView twitterImageView = FindViewById<ImageView>(Resource.Id.imageTwitter);
			twitterImageView.Click+= (sender, e) => {
				Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://www.twitter.com/broditech"));
				StartActivity(intent);
			};

			ImageView broditechImageView = FindViewById<ImageView>(Resource.Id.imageBroditech);
			broditechImageView.Click+= (sender, e) => {
				Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://www.broditech.com"));
				StartActivity(intent);
			};

			#if YESTER_LITE
			Button buttonFullVersion = FindViewById<Button>(Resource.Id.button_about_lite_full_link);
			buttonFullVersion.Click += (sender, e) => {
				Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(Utils.YesterFullMarketUrl()));
				StartActivity(intent);
			};
			#endif
		}
	}
}

