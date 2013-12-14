using System;
using Android.App;
using Android.Content;
using Android.Views;

namespace iSeconds.Droid
{
	public static class TutorialDialogFactory
	{

		public static Dialog CreateDialog(Context context, LayoutInflater inflater, Action doneAction)
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(context);
			builder.SetView(inflater.Inflate(Resource.Layout.TutorialView, null));
			builder.SetPositiveButton(Resource.String.tutorial_done, (sender, args) => {
				doneAction.Invoke();
			});
			return builder.Create();
		}
	}
}

