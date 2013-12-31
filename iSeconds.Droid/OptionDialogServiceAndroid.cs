using System;
using iSeconds.Domain;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.Views;

namespace iSeconds.Droid
{
	public class OptionDialogServiceAndroid : IOptionsDialogService
	{
		private ActivityTracker tracker = null;

		public OptionDialogServiceAndroid (ActivityTracker tracker)
		{
			this.tracker = tracker;
		}

		public void ShowModal (OptionsList options)
		{
			var builder = new AlertDialog.Builder(tracker.GetCurrentActivity());
			builder.SetTitle(string.Empty);
			builder.SetItems(options.ListNames(), (sender, eventArgs) => options.EntryClicked.Execute(eventArgs.Which));
			builder.Create().Show();
		}

		public void ShowMessage(string msg, Action callback)
		{
			new AlertDialog.Builder(tracker.GetCurrentActivity())
				.SetTitle (string.Empty)
				.SetMessage (msg)
				.SetPositiveButton(Resource.String.ok, delegate { callback.Invoke(); })
				.Show ();
		}

		public void AskForConfirmation(string msg, Action userConfirmedCallback, Action userCanceledCallback )
		{
			new AlertDialog.Builder(tracker.GetCurrentActivity())
				.SetTitle(String.Empty)
					.SetMessage(msg)
					.SetPositiveButton(Resource.String.ok, delegate	{
						userConfirmedCallback.Invoke();
					})
					.SetNegativeButton(Resource.String.cancel, delegate {
						userCanceledCallback.Invoke();
					})
					.Show();
		}

		public void ShowTutorial(Action doneAction)
		{
			Activity activity = tracker.GetCurrentActivity();
			AlertDialog.Builder builder = new AlertDialog.Builder(activity);
			builder.SetTitle(Resource.String.tutorial_welcome);
			builder.SetView(activity.LayoutInflater.Inflate(Resource.Layout.TutorialView, null));
			builder.SetPositiveButton(Resource.String.tutorial_done, (sender, args) => {
				if (doneAction != null)
					doneAction.Invoke();
			});

			Dialog dialog = builder.Create();
			dialog.ShowEvent += (sender, e) => {
				TextViewUtil.ChangeForDefaultFont(dialog.FindViewById<TextView>(Resource.Id.textViewMsg1), activity, 18f);
				TextViewUtil.ChangeForDefaultFont(dialog.FindViewById<TextView>(Resource.Id.textViewMsg2), activity, 18f);
				TextViewUtil.ChangeForDefaultFont(dialog.FindViewById<TextView>(Resource.Id.textViewOption1), activity, 18f);
				TextViewUtil.ChangeForDefaultFont(dialog.FindViewById<TextView>(Resource.Id.textViewOption2), activity, 18f);
				TextViewUtil.ChangeForDefaultFont(dialog.FindViewById<TextView>(Resource.Id.textViewOption3), activity, 18f);
				TextViewUtil.ChangeForDefaultFont(dialog.FindViewById<TextView>(Resource.Id.textViewOption4), activity, 18f);
				TextViewUtil.ChangeForDefaultFont(dialog.FindViewById<TextView>(Resource.Id.textViewOption5), activity, 18f);
				TextViewUtil.ChangeForDefaultFont(dialog.FindViewById<TextView>(Resource.Id.textViewHold), activity, 18f);
				TextViewUtil.ChangeForDefaultFont(dialog.FindViewById<TextView>(Resource.Id.textViewFling), activity, 18f);
				TextViewUtil.ChangeForDefaultFont(dialog.FindViewById<TextView>(Resource.Id.textViewMsg3), activity, 18f);
				TextViewUtil.ChangeForDefaultFont(dialog.FindViewById<TextView>(Resource.Id.textViewMsg4), activity, 18f);

				TextViewUtil.ChangeForDefaultFont(dialog.FindViewById<TextView>(Resource.Id.textViewAccessAbout), activity, 18f);
			};

			dialog.Show();
		}
	}
}

