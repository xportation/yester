using System;
using Android.Content;
using Android.App;

namespace iSeconds.Droid
{
   [BroadcastReceiver]
//   [IntentFilter(new string[]{VideoUploadService.VideoUploadAction}, Priority = (int)IntentFilterPriority.LowPriority)]
   public class VideoUploadNotificationReceiver : BroadcastReceiver
   {
      public VideoUploadNotificationReceiver()
      {
      }

      public override void OnReceive(Context context, Intent intent)
      {
         var nMgr = (NotificationManager)context.GetSystemService (Context.NotificationService);
         var notification = new Notification (Resource.Drawable.ic_logo, "Your movies are ready!!");
         var pendingIntent = PendingIntent.GetActivity (context, 0, new Intent (context, typeof(ShareActivity)), 0);
         notification.SetLatestEventInfo (context, "Videos Legal", "Your movies are ready!!", pendingIntent);
         nMgr.Notify (0, notification);
      }
   }
}

