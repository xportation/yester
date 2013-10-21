using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace iSeconds.Droid
{
   class VideoUploadServiceConnection : Java.Lang.Object, IServiceConnection
   {
      private VideoUploadService.VideoUploadServiceBinder binder= null;

      public VideoUploadService.VideoUploadServiceBinder Binder
      {
         get { return binder; }
      }

      public delegate void ServiceConnected();
      public event ServiceConnected onServiceConnected;

      public delegate void ServiceDisconnected();
      public event ServiceDisconnected onServiceDisconnected;

      public void OnServiceConnected (ComponentName name, IBinder service)
      {
         binder = service as VideoUploadService.VideoUploadServiceBinder;
         if (onServiceConnected != null)
            onServiceConnected();
      }

      public void OnServiceDisconnected (ComponentName name)
      {
         if (onServiceDisconnected != null)
            onServiceDisconnected();
      }
   }

   [Service]
//   [IntentFilter(new String[]{VideoUploadService.VideoUploadServiceDescription})]
   class VideoUploadService : Service
	{
		IBinder binder;
      Session session= null;

//		public const string VideoUploadAction = "VideoUpload";
      public const string VideoUploadServiceDescription = "iSeconds.Droid.VideoUploadService";

//		protected override void OnHandleIntent(Intent intent)
//		{
//			var uploadIntent = new Intent (VideoUploadAction); 
//
//			SendOrderedBroadcast (uploadIntent, null);
//		}

		public override IBinder OnBind (Intent intent)
		{
			binder = new VideoUploadServiceBinder (this);
			return binder;
		}

      public override void OnCreate()
      {
         base.OnCreate();
         Toast.MakeText(this, "Servico criado amigo", ToastLength.Long).Show();
      }

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			return StartCommandResult.Sticky;
		}

      public override void OnDestroy()
      {
			Toast.MakeText(this, "Servico destruido amigo", ToastLength.Long).Show();

         base.OnDestroy();
			StopSelf();
      }

      public void Share(IList<string> videosPath, string resultPath)
      {
         Toast.MakeText(this, "Oi amiguinho, aqui eh o gorila suquinho... comecamos a concatenar", ToastLength.Long).Show();

         if (session == null)
            session = new Session(1);

         session.StartConcatenation(videosPath);
			session.onUploadConcluded+= () => 
			{
				NotificationManager notificationManager =	(NotificationManager) this.GetSystemService(Context.NotificationService);
				Notification notification = new Notification(Resource.Drawable.Icon, "Concatenation concluded");
				PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(ShareActivity)), 0);
				notification.SetLatestEventInfo(this, "Concatenation concluded", "Concatenation concluded mano.", pendingIntent);
				notificationManager.Notify(0, notification);
			};
      }

      public string GetDownloadLink()
      {
         Toast.MakeText(this, "Download link", ToastLength.Long).Show();
         return "cuidado com a cuca, que a cuca te pega";
      }


		public class VideoUploadServiceBinder : Binder
		{
			VideoUploadService service;

			public VideoUploadServiceBinder(VideoUploadService service)
			{
				this.service = service;
			}

			public VideoUploadService GetVideoUploadService()
			{
				return service;
			}
		}
	}
}

