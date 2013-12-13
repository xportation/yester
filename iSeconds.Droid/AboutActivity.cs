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
			configureButtonTutorial();
		}

		void configureFonts()
		{
			TextView aboutTextView= FindViewById<TextView>(Resource.Id.textAbout);
			TextView siteTextView= FindViewById<TextView>(Resource.Id.textSite);
			Button buttonTutorial= FindViewById<Button>(Resource.Id.buttonTutorial);
			TextViewUtil.ChangeForDefaultFont(aboutTextView,this,18f);
			TextViewUtil.ChangeForDefaultFont(siteTextView,this,18f);
			TextViewUtil.ChangeForDefaultFont(buttonTutorial,this,18f);
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

		void configureButtonTutorial()
		{
			Button buttonTutorial = FindViewById<Button>(Resource.Id.buttonTutorial);
			buttonTutorial.Click+= (sender, e) => {
				this.ShowDialog(0);
			};
		}

		protected override Dialog OnCreateDialog(int dialogId)
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(this);

			LayoutInflater inflater = this.LayoutInflater;
			builder.SetView(inflater.Inflate(Resource.Layout.TutorialView, null));
			return builder.Create();
		}
	}
}

