using System;
using System.IO;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Widget;
using Android.OS;
using iSeconds.Domain;
using Android.Content;
using Android.Views;

namespace iSeconds
{
	[Activity (Label = "iSeconds", MainLauncher = true, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.Orientation)]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);
		}


		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.MenuItems, menu);
			return true;
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			// Handle item selection
			switch (item.ItemId) {
			case Resource.Id.menu_media_sandbox:
				this.StartActivity(typeof(iSeconds.MediaSandboxActivity));
				return true;			
			case Resource.Id.menu_timelines:
				this.StartActivity(typeof(iSeconds.UserTimelinesActivity));
				return true;
			default:
				return base.OnOptionsItemSelected(item);
			}
		}
	
	}

		
}