using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace iSeconds.Droid
{
	public static class TutorialDialogFactory
	{

		public static Dialog CreateDialog(Activity activity, Action doneAction)
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(activity);
			builder.SetView(activity.LayoutInflater.Inflate(Resource.Layout.TutorialView, null));
			builder.SetPositiveButton(Resource.String.tutorial_done, (sender, args) => {
				doneAction.Invoke();
			});
			return builder.Create();
		}

		public static void ChangeFonts(Dialog dialog, Context context)
		{
			TextViewUtil.ChangeForDefaultFont(dialog.FindViewById<TextView>(Resource.Id.textViewWelcome), context, 26f);
			TextViewUtil.ChangeForDefaultFont(dialog.FindViewById<TextView>(Resource.Id.textViewMsg1), context, 18f);
			TextViewUtil.ChangeForDefaultFont(dialog.FindViewById<TextView>(Resource.Id.textViewOption1), context, 18f);
			TextViewUtil.ChangeForDefaultFont(dialog.FindViewById<TextView>(Resource.Id.textViewOption2), context, 18f);
			TextViewUtil.ChangeForDefaultFont(dialog.FindViewById<TextView>(Resource.Id.textViewOption3), context, 18f);
			TextViewUtil.ChangeForDefaultFont(dialog.FindViewById<TextView>(Resource.Id.textViewOption4), context, 18f);
			TextViewUtil.ChangeForDefaultFont(dialog.FindViewById<TextView>(Resource.Id.textViewOption5), context, 18f);
			TextViewUtil.ChangeForDefaultFont(dialog.FindViewById<TextView>(Resource.Id.textViewHold), context, 18f);
			TextViewUtil.ChangeForDefaultFont(dialog.FindViewById<TextView>(Resource.Id.textViewFling), context, 18f);
			TextViewUtil.ChangeForDefaultFont(dialog.FindViewById<TextView>(Resource.Id.textViewMsg2), context, 18f);
			TextViewUtil.ChangeForDefaultFont(dialog.FindViewById<TextView>(Resource.Id.textViewAccessAbout), context, 18f);
		}
	}
}

