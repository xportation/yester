using System;
using Android.Content;
using Android.App;

namespace iSeconds.Droid
{
	[BroadcastReceiver]
	[IntentFilter(new string[]{FFMpegService.ConcatFinishedIntent}, Priority = (int)IntentFilterPriority.LowPriority)]
	public class CompileFinishedNotificationReceiver : BroadcastReceiver
	{
		public CompileFinishedNotificationReceiver ()
		{
		}

		public override void OnReceive (Context context, Intent intent)
		{
			var nMgr = (NotificationManager)context.GetSystemService (Context.NotificationService);
			var notification = new Notification (Resource.Drawable.Icon, "Compilation is finished");
			var pendingIntent = PendingIntent.GetActivity (context, 0, new Intent (context, typeof(CompilationActivity)), 0);
			notification.SetLatestEventInfo (context, "Compilation finished", "Your compilation is now available", pendingIntent);
			nMgr.Notify (0, notification);
		}
	}

}

