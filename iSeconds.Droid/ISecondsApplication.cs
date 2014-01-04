
using System;
using Android.App;
using iSeconds.Domain;
using Android.Runtime;
using iSeconds.Droid;
using System.Collections.Generic;
using Android.Content;
using iSeconds.Domain.Framework;

namespace iSeconds.Droid
{
   [Application(Debuggable=true)]
   public class ISecondsApplication : Application
   {
      private UserService userService = null;
      private IRepository repository = null;
      private IMediaService mediaService = null;
      private ActivityTracker activityTracker = null;
      private INavigator navigator = null;
      private IPathService pathService = null;
		private IOptionsDialogService optionsDialogService = null;
		private I18nService i18nService = null;

      public ISecondsApplication (IntPtr javaReference, JniHandleOwnership transfer)
         : base(javaReference, transfer)
      {
			pathService = new PathServiceAndroid();
			repository = new ISecondsDB (pathService.GetDbPath());

			i18nService = new I18nServiceAndroid(this.BaseContext);
			userService = new UserService (repository, i18nService);
			if (!userService.Login ("user", "password"))
				userService.CreateUser ("user");
         
			activityTracker = new ActivityTracker ();
			mediaService = new MediaServiceAndroid (this.activityTracker, repository, pathService.GetMediaPath(), userService.CurrentUser);         

			navigator = new INavigator ();
         navigator.RegisterNavigation ("day_options", new AndroidPresenter (this.activityTracker, typeof(DayOptionsActivity)));            
         navigator.RegisterNavigation ("timeline_options", new AndroidPresenter (this.activityTracker, typeof(TimelineOptionsActivity)));
         navigator.RegisterNavigation ("timeline_view", new AndroidPresenter (this.activityTracker, typeof(TimelineActivity)));
			navigator.RegisterNavigation ("settings_view", new AndroidPresenter(this.activityTracker, typeof(SettingsActivity)));
			navigator.RegisterNavigation ("about_view", new AndroidPresenter(this.activityTracker, typeof(AboutActivity)));
			navigator.RegisterNavigation ("video_player", new AndroidPresenter(this.activityTracker, typeof(VideoPlayerActivity)));
			navigator.RegisterNavigation ("compilations_view", new AndroidPresenter(this.activityTracker, typeof(CompilationActivity)));
			navigator.RegisterNavigation ("single_shot_video_player", new AndroidPresenter(this.activityTracker, typeof(SingleShotVideoPlayerActivity)));

			optionsDialogService = new OptionDialogServiceAndroid (activityTracker);
      }

      public UserService GetUserService ()
      {
         return userService;
      }

      public IRepository GetRepository ()
      {
         return repository;
      }

      public IMediaService GetMediaService ()
      {
         return mediaService;
      }

      public ActivityTracker GetActivityTracker ()
      {
         return this.activityTracker;
      }

      public INavigator GetNavigator ()
      {
         return navigator;
      }

      public IPathService GetPathService ()
      {
         return pathService;
      }

		public IOptionsDialogService GetOptionsDialogService ()
		{
			return optionsDialogService;
		}

		public I18nService GetI18nService()
		{
			return i18nService;
		}
   }

}

