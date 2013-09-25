using System;
using iSeconds.Domain;
using Android.App;
using Android.Content;

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

	}
}

