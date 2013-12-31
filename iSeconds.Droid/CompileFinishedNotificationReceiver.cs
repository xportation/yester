using System;
using Android.Content;
using Android.App;

namespace iSeconds.Droid
{
	[BroadcastReceiver]
	[IntentFilter(new string[]{FFMpegService.ConcatFinishedIntent}, Priority = (int)IntentFilterPriority.HighPriority)]
	public class CompileFinishedNotificationReceiver : BroadcastReceiver
	{
		public CompileFinishedNotificationReceiver ()
		{
		}

		public override void OnReceive (Context context, Intent intent)
		{
			var nMgr = (NotificationManager)context.GetSystemService (Context.NotificationService);

			string title = context.Resources.GetString(Resource.String.compilation_notification_title);
			string description = context.Resources.GetString(Resource.String.compilation_notification_description);

			var notification = new Notification (Resource.Drawable.Icon, title);
			var pendingIntent = PendingIntent.GetActivity (context, 0, new Intent (context, typeof(CompilationActivity)), 0);
			notification.SetLatestEventInfo (context, title, description, pendingIntent);
			notification.Flags = NotificationFlags.AutoCancel; // isso eh para que a notificaçao saia quando o usuario clickar nela
			nMgr.Notify (1, notification);
		}
	}

}

