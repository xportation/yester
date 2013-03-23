
using System;
using Android.App;
using iSeconds.Domain;
using Android.Runtime;
using iSeconds.Droid;
using System.Collections.Generic;
using Android.Content;

namespace iSeconds
{
	[Application(Debuggable=true, Label="insert label here")]
	public class ISecondsApplication : Application
	{
		private UserService userService = null;
		private IRepository repository = null;
        private IMediaService mediaService = null;

        private ActivityTracker activityTracker = null;


		public ISecondsApplication (IntPtr javaReference, JniHandleOwnership transfer)
			: base(javaReference, transfer)
		{
			userService = new UserService ();

			repository = new ISecondsDB (ISecondsDB.DatabaseFilePath);

            activityTracker = new ActivityTracker();

            mediaService = new MediaServiceAndroid(this.activityTracker);

			userService.CurrentUser = new User ("test");
		}

		public override void OnCreate()
		{
			base.OnCreate();
            
			//userService.ActualUser.CreateTimeline("test");
		}

		public UserService GetUserService ()
		{
			return userService;
		}

        public IRepository GetRepository()
        {
            return repository;
        }

        public IMediaService GetMediaService()
        {
            return mediaService;
        }

        public ActivityTracker GetActivityTracker()
        {
            return this.activityTracker;
        }
    }

}

