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
		private const int TutorialDialogId = 20;

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
			TextView stayInTouchTextView= FindViewById<TextView>(Resource.Id.textStayInTouch);
			TextView siteTextView= FindViewById<TextView>(Resource.Id.textSite);
			Button buttonTutorial= FindViewById<Button>(Resource.Id.buttonTutorial);
			TextViewUtil.ChangeForDefaultFont(aboutTextView,this,18f);
			TextViewUtil.ChangeForDefaultFont(stayInTouchTextView,this,18f);
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
				this.ShowDialog(TutorialDialogId);
			};
		}

		protected override Dialog OnCreateDialog(int dialogId)
		{
			if (dialogId == TutorialDialogId)
				return TutorialDialogFactory.CreateDialog(this, () => {});

			return base.OnCreateDialog(dialogId);
		}

		protected override void OnPrepareDialog(int dialogId, Dialog dialog)
		{
			if (dialogId == TutorialDialogId)
				TutorialDialogFactory.ChangeFonts(dialog, this);
		}
	}
}

