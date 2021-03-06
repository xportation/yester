using System;
using iSeconds.Domain;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.Views;
using System.Threading;
using System.Diagnostics;

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
				.SetPositiveButton(Resource.String.ok, delegate { 
					if (callback != null)
						callback.Invoke(); 
				})
				.Show ();
		}

		#if YESTER_LITE
		public void ShowMessageLite(string msg, Action callback)
		{
			Activity activity = tracker.GetCurrentActivity();
			AlertDialog.Builder builder = new AlertDialog.Builder(activity);
			View layout = activity.LayoutInflater.Inflate(Resource.Layout.MessageBoxLite, null);
			builder.SetView(layout);
			builder.SetPositiveButton(Resource.String.ok, delegate { 
				if (callback != null)
					callback.Invoke(); 
			});

			Dialog dialog = builder.Create();
			dialog.ShowEvent += (sender, e) => {
				TextView message = dialog.FindViewById<TextView>(Resource.Id.messagebox_lite_message);
				message.Text = msg;

				Button buttonFullVersion = dialog.FindViewById<Button>(Resource.Id.button_messagebox_lite_full_link);
				buttonFullVersion.Click += (sender2, e2) => {
					Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(Utils.YesterFullMarketUrl()));
					activity.StartActivity(intent);

					dialog.Dismiss();
				};
			};

			dialog.Show();
		}
		#endif

		public void AskForConfirmation(string msg, Action userConfirmedCallback, Action userCanceledCallback )
		{
			new AlertDialog.Builder(tracker.GetCurrentActivity())
				.SetTitle(String.Empty)
				.SetMessage(msg)
				.SetPositiveButton(Resource.String.ok, delegate	{
					if (userConfirmedCallback != null)
						userConfirmedCallback.Invoke();
				})
				.SetNegativeButton(Resource.String.cancel, delegate {
					if (userCanceledCallback != null)
						userCanceledCallback.Invoke();
				})
				.Show();
		}

		public void AskForCompilationNameAndDescription(string defaultName, string defaultDescription, 
			Action<string, string> userConfirmedCallback, Action userCanceledCallback)
		{
			Activity activity = tracker.GetCurrentActivity();
			AlertDialog.Builder builder = new AlertDialog.Builder(activity);
			View layout = activity.LayoutInflater.Inflate(Resource.Layout.CompilationViewDescription, null);
			builder.SetView(layout);
			builder.SetPositiveButton(Resource.String.ok, (sender, args) => {
				if (userConfirmedCallback != null) {
					EditText editName = layout.FindViewById<EditText>(Resource.Id.compilationNameEdit);
					EditText editDescription = layout.FindViewById<EditText>(Resource.Id.compilationDescriptionEdit);
					userConfirmedCallback.Invoke(editName.Text, editDescription.Text);
				}
			});

			builder.SetNegativeButton(Resource.String.cancel, (sender, args) => {
				if (userCanceledCallback != null)
					userCanceledCallback.Invoke();
			});

			Dialog dialog = builder.Create();
			dialog.ShowEvent += (sender, e) => {
				EditText editName = dialog.FindViewById<EditText>(Resource.Id.compilationNameEdit);
				editName.Text = defaultName;

				EditText editDescription = dialog.FindViewById<EditText>(Resource.Id.compilationDescriptionEdit);
				editDescription.Text = defaultDescription;
			};

			dialog.Show();
		}

		public void AskForTimelineNameAndDescription(string defaultName, string defaultDescription, 
			Action<string, string> userConfirmedCallback, Action userCanceledCallback)
		{
			Activity activity = tracker.GetCurrentActivity();
			AlertDialog.Builder builder = new AlertDialog.Builder(activity);
			View layout = activity.LayoutInflater.Inflate(Resource.Layout.TimelineOptionsEditTimeline, null);
			builder.SetView(layout);

			builder.SetPositiveButton(Resource.String.ok,
				(sender, args) =>
				{
					if (userConfirmedCallback != null) {
						EditText editName = layout.FindViewById<EditText>(Resource.Id.timelineName);
						EditText editDescription = layout.FindViewById<EditText>(Resource.Id.timelineDescription);
						userConfirmedCallback.Invoke(editName.Text, editDescription.Text);
					}
				}
			);

			builder.SetNegativeButton(Resource.String.cancel, (sender, args) => {
				if (userCanceledCallback != null)
					userCanceledCallback.Invoke();
			});

			Dialog dialog = builder.Create();
			dialog.ShowEvent += (sender, e) => {
				EditText editName = dialog.FindViewById<EditText>(Resource.Id.timelineName);
				editName.Text = defaultName;

				EditText editDescription = dialog.FindViewById<EditText>(Resource.Id.timelineDescription);
				editDescription.Text = defaultDescription;
			};

			dialog.Show();
		}

		public void ShowProgressDialog(Action actionToPerform, string message)
		{
			Debug.Assert(actionToPerform != null);

			Activity activity = tracker.GetCurrentActivity();
			var progressDialog = new ProgressDialog(activity);
			progressDialog.Indeterminate = true;
			progressDialog.SetMessage(message);
			progressDialog.Show();

			ThreadPool.QueueUserWorkItem(d => {
				actionToPerform.Invoke();
				activity.RunOnUiThread(() => progressDialog.Dismiss());
			});
		}
	}
}

