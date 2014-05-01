using System;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using iSeconds.Domain;

namespace iSeconds.Droid
{
	public static class AppRater
	{
		private const int DAYS_UNTIL_PROMPT = 10;
		private const int LAUNCHES_UNTIL_PROMPT = 15;

		public static void AppLaunched(Activity context) 
		{
			ISharedPreferences prefs = context.GetSharedPreferences("rater", 0);
			if (prefs.GetBoolean("dontshowagain", false)) { return ; }

			ISharedPreferencesEditor editor = prefs.Edit();

			// Increment launch counter
			long launch_count = prefs.GetLong("launch_count", 0) + 1;
			editor.PutLong("launch_count", launch_count);

			// Get date of first launch
			long date_firstLaunch = prefs.GetLong("date_firstlaunch", 0);
			if (date_firstLaunch == 0) {
				date_firstLaunch = Java.Lang.JavaSystem.CurrentTimeMillis();
				editor.PutLong("date_firstlaunch", date_firstLaunch);
			}

			// Wait at least n days before opening
			if (launch_count >= LAUNCHES_UNTIL_PROMPT) {
				if (Java.Lang.JavaSystem.CurrentTimeMillis() >= date_firstLaunch + 
					(DAYS_UNTIL_PROMPT * 24 * 60 * 60 * 1000)) {
					showRateDialog(context, editor);
				}
			}

			editor.Commit();
		}   

		private static void showRateDialog(Activity context, ISharedPreferencesEditor editor) 
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(context);
			builder.SetIcon(Resource.Drawable.Icon);
			builder.SetTitle(Resource.String.rating_title);
			builder.SetMessage(Resource.String.rating_message);
			builder.SetPositiveButton(Resource.String.rating_rate, delegate { 
				if (editor != null) {
					editor.PutBoolean("dontshowagain", true);
					editor.Commit();
				}
				context.StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse(Utils.YesterFullMarketUrl())));
			});
			builder.SetNeutralButton(Resource.String.rating_later, delegate {});
			builder.SetNegativeButton(Resource.String.rating_no_thanks, delegate {
				if (editor != null) {
					editor.PutBoolean("dontshowagain", true);
					editor.Commit();
				}
			});

			Dialog dialog = builder.Create();
			dialog.Show();
		}
	}
}

