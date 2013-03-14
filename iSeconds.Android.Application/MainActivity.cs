

using Android.App;
using iSeconds.Domain;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Content;
using Android.Views;

namespace iSeconds
{
	[Activity (Label = "iSeconds", MainLauncher = true, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.Orientation)]
	public class MainActivity : FragmentActivity
	{
		UserService userService = null;
		User actualUser = null;
		LinearLayout layout = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			layout = (LinearLayout)this.FindViewById(Resource.Id.mainLayout);

			userService = ((ISecondsApplication)this.Application).GetUserService ();
			this.SupportFragmentManager.BeginTransaction()
				.Add(Resource.Id.mainLayout, new TimelineFragment())
					.Commit();

			actualUser = userService.CurrentUser;
			
			if (actualUser != null) 
			{
				if (actualUser.TimelineCount == 0)
				{
					this.StartActivityForResult(typeof(iSeconds.UserTimelinesActivity), ISecondsConstants.TIMELINE_CHOOSER_RESULT);
				} 
			} 
			//else redirecionar para login...		
		}


		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == ISecondsConstants.TIMELINE_CHOOSER_RESULT) 
			{
				if (resultCode == Result.Ok) 
				{
					Toast toast = Toast.MakeText(this, actualUser.ActualTimeline.Name, ToastLength.Short);
					toast.Show();

					//invalidateTimeline();
				}
			}

			base.OnActivityResult(requestCode, resultCode, data);
		}

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MenuItems, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // Handle item selection
            switch (item.ItemId)
            {
                case Resource.Id.menu_media_sandbox:
                    this.StartActivity(typeof(iSeconds.MediaSandboxActivity));
                    return true;
                case Resource.Id.menu_timelines:
                    this.StartActivity(typeof(iSeconds.UserTimelinesActivity));
                    return true;
			case Resource.Id.menu_inspect_db:
					this.StartActivity(typeof(iSeconds.InspectDbActivity));
					return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }




	}

}