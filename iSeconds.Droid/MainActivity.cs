using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using iSeconds.Domain;

namespace iSeconds.Droid
{
	[Activity (Label = "iSeconds.Droid", MainLauncher = true)]
	public class MainActivity : Activity
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


