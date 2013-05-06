using System;
using Android.App;
using Android.OS;
using iSeconds.Domain;
using Android.Views;

namespace iSeconds.Droid
{
	[Activity (Label= "Memories", MainLauncher = true, Theme = "@style/Theme.Splash", NoHistory = true)]
	public class SplashActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			UserService userService = ((ISecondsApplication)this.Application).GetUserService();
			
			if (userService.CurrentUser != null)
				this.StartActivity(typeof(HomeActivity));
			// else
			// this.StartActivity<LoginActivity>();
		}
	}
}
