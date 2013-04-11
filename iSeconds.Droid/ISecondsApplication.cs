
using System;
using Android.App;
using iSeconds.Domain;
using Android.Runtime;
using iSeconds.Droid;
using System.Collections.Generic;
using Android.Content;
using iSeconds.Domain.Framework;

namespace iSeconds
{
	[Application(Debuggable=true, Label="")]
	public class ISecondsApplication : Application
	{
		private UserService userService = null;
		private IRepository repository = null;
        private IMediaService mediaService = null;

        private ActivityTracker activityTracker = null;

        private INavigator navigator = null;


		public ISecondsApplication (IntPtr javaReference, JniHandleOwnership transfer)
			: base(javaReference, transfer)
		{
			userService = new UserService ();

			repository = new ISecondsDB (ISecondsDB.DatabaseFilePath);

            activityTracker = new ActivityTracker();

            mediaService = new MediaServiceAndroid(this.activityTracker);

            navigator = new INavigator();

            navigator.RegisterNavigation("day_options", new AndroidPresenter(this.activityTracker, typeof(DayOptionsActivity)));
            
            navigator.RegisterNavigation("timeline_options", new AndroidPresenter(this.activityTracker, typeof(TimelineOptionsActivity)));

            navigator.RegisterNavigation("homeview", new AndroidPresenter(this.activityTracker, typeof(HomeActivity)));

            // ---
			userService.CurrentUser = new User ("test", repository);
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

        public INavigator GetNavigator()
        {
            return navigator;
        }
    }

}

