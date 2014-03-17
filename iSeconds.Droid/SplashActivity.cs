using System;
using Android.App;
using Android.OS;
using iSeconds.Domain;
using Android.Views;
using Android.Widget;
using iSeconds.Domain.Framework;
using Android.Telephony;
using Android.Accounts;
using Android.Content;

namespace iSeconds.Droid
{
	[Activity (Label= "@string/app_name", MainLauncher = true, Theme = "@style/Theme.Splash", NoHistory = true)]
   public class SplashActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
         base.OnCreate (bundle);

         ISecondsApplication application = this.Application as ISecondsApplication;
         IPathService pathService = application.GetPathService();
					
         if (pathService != null && pathService.IsGood()) {
            try {
               this.StartActivity(typeof(TimelineActivity));
				} catch (Exception) {
               this.Finish();
            }
         } else {
            showMessage(Resources.GetString(Resource.String.sd_cad_not_available_error_message));
         }
		}

      private void showMessage(string message)
		{
			new AlertDialog.Builder(this)
				.SetTitle (string.Empty)
            .SetMessage(message)
				.SetPositiveButton(Resource.String.ok, delegate { this.Finish(); })
				.Show ();
		}
	}
}

