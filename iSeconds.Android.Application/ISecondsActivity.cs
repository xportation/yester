
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace iSeconds
{
	[Activity (Label = "ISecondsActivity")]			
	public class ISecondsActivity : FragmentActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
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

