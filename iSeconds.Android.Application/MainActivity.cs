

using Android.App;
using iSeconds.Domain;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Content;

namespace iSeconds
{
	[Activity (Label = "iSeconds", MainLauncher = true, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.Orientation)]
	public class MainActivity : ISecondsActivity
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


			actualUser = userService.ActualUser;
			
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


	}

}