using System;
using iSeconds.Domain;
using Android.App;

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
			builder.SetItems(options.ListNames(), (sender, eventArgs) => options.DayEntryClicked.Execute(eventArgs.Which));
			builder.Create().Show();
		}

	}
}

