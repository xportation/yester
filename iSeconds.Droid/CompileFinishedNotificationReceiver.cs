using System;
using Android.Content;
using Android.App;
using iSeconds.Domain;

namespace iSeconds.Droid
{
	[BroadcastReceiver]
//	[IntentFilter(new string[]{FFMpegService.ConcatFinishedIntent}, Priority = (int)IntentFilterPriority.HighPriority)]
	public class CompileFinishedNotificationReceiver : BroadcastReceiver
	{
		private User user = null;

		//Its necessary a default contructor :(
		public CompileFinishedNotificationReceiver ()
		{
		}

		public CompileFinishedNotificationReceiver (User user)
		{
			this.user = user;
		}

		public override void OnReceive (Context context, Intent intent)
		{
			var nMgr = (NotificationManager)context.GetSystemService (Context.NotificationService);

			bool hasErrors = intent.GetBooleanExtra("ffmpeg.concat.result.errors", true);
			string compilationResultFilename= intent.GetStringExtra("ffmpeg.concat.result");

			int iconId;
			string title;
			string description;

			if (hasErrors) {
				iconId = Resource.Drawable.ic_action_error;
				title = context.Resources.GetString(Resource.String.compilation_error_title);
				description = context.Resources.GetString(Resource.String.compilation_error_description);

				user.DeleteCompilation(compilationResultFilename);
			} else {
				iconId = Resource.Drawable.Icon;
				title = context.Resources.GetString(Resource.String.compilation_notification_title);
				description = 	context.Resources.GetString(Resource.String.compilation_notification_description);
				user.SetCompilationDone(compilationResultFilename, true);
			}

			var notification = new Notification (iconId, title);
			var pendingIntent = PendingIntent.GetActivity (context, 0, new Intent (context, typeof(CompilationActivity)), 0);
			notification.SetLatestEventInfo (context, title, description, pendingIntent);
			notification.Flags = NotificationFlags.AutoCancel; // isso eh para que a notificaçao saia quando o usuario clickar nela
			nMgr.Notify (1, notification);
		}
	}

}

