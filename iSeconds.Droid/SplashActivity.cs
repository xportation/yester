using System;
using Android.App;
using Android.OS;
using iSeconds.Domain;
using Android.Views;
using Android.Widget;
using iSeconds.Domain.Framework;

namespace iSeconds.Droid
{
	[Activity (Label= "@string/app_name", MainLauncher = true, Theme = "@style/Theme.Splash", NoHistory = true)]
	public class SplashActivity : ISecondsActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
         try {
				navigator.NavigateTo("timeline_view", new Args());
         } catch (Exception) {
            this.Finish();
         }
		}
	}
}

