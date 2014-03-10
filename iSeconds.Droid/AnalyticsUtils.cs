using System;
using Android.App;
using Google.Analytics.Tracking;

namespace iSeconds.Droid
{
   public static class AnalyticsUtils
   {
      public static void LogButtonEvent(Activity activity, string label )
      {
         var easyTracker = EasyTracker.GetInstance(activity);

         var gaEvent = MapBuilder.CreateEvent ("UI", "button_press", label, null).Build();
         easyTracker.Send(gaEvent);
      }
   }
}

