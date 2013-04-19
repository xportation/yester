
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
      private IPathService pathService = null;

      public ISecondsApplication (IntPtr javaReference, JniHandleOwnership transfer)
         : base(javaReference, transfer)
      {
         userService = new UserService ();



         activityTracker = new ActivityTracker ();

         pathService = new PathServiceAndroid();

         repository = new ISecondsDB (pathService.GetDbPath());

         mediaService = new MediaServiceAndroid (this.activityTracker, pathService.GetMediaPath());

         navigator = new INavigator ();

         navigator.RegisterNavigation ("day_options", new AndroidPresenter (this.activityTracker, typeof(DayOptionsActivity)));
            
         navigator.RegisterNavigation ("timeline_options", new AndroidPresenter (this.activityTracker, typeof(TimelineOptionsActivity)));

         navigator.RegisterNavigation ("homeview", new AndroidPresenter (this.activityTracker, typeof(HomeActivity)));

         // ---
         userService.CurrentUser = new User ("test", repository);
      }

      public override void OnCreate ()
      {
         base.OnCreate ();
            
         //userService.ActualUser.CreateTimeline("test");
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
   }

}

