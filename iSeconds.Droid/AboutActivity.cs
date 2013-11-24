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

namespace iSeconds.Droid
{
	[Activity (Label = "AboutActivity")]			
	public class AboutActivity : ISecondsActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			this.RequestWindowFeature(WindowFeatures.NoTitle);
			this.SetContentView(Resource.Layout.AboutView);

			configureActionBar(true, "");
			configureFonts();
			configureHyperlinks();
		}

		void configureFonts()
		{
			TextView aboutTextView= FindViewById<TextView>(Resource.Id.textAbout);
			TextView siteTextView= FindViewById<TextView>(Resource.Id.textSite);
			TextViewUtil.ChangeForDefaultFont(aboutTextView,this,18f);
			TextViewUtil.ChangeForDefaultFont(siteTextView,this,18f);
		}

		void configureHyperlinks()
		{
			ImageView facebookImageView = FindViewById<ImageView>(Resource.Id.imageFacebook);
			facebookImageView.Click+= (sender, e) => {
				Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://www.facebook.com/broditech"));
				StartActivity(intent);
			};

			ImageView twitterImageView = FindViewById<ImageView>(Resource.Id.imageTwitter);
			twitterImageView.Click+= (sender, e) => {
				Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://www.twitter.com/broditech"));
				StartActivity(intent);
			};
		}
	}
}

