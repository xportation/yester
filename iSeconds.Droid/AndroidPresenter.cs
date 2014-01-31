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
using iSeconds.Domain.Framework;
using iSeconds.Domain;

namespace iSeconds.Droid
{
   public class AndroidPresenter : IPresenter
   {
      private ActivityTracker tracker = null;
      private Type activityType = null;
		private bool animating = true;

      public AndroidPresenter (ActivityTracker tracker, Type activityType)
      {
         this.tracker = tracker;
         this.activityType = activityType;
      }

		public void Show (Args args)
      {
         Activity actual = this.tracker.GetCurrentActivity ();

         Intent intent = new Intent (actual, activityType);
         foreach (var arg in args.GetArgs()) {
            intent.PutExtra (arg.Key, arg.Value);
         }

			if (!animating)
				intent.AddFlags(ActivityFlags.NoAnimation);
            
         actual.StartActivity (intent);
      }

		public void Close ()
      {
         Activity actual = this.tracker.GetCurrentActivity ();
         actual.Finish();
			actual.OverridePendingTransition(0, 0);
      }

		public void DisableAnimation()
		{
			animating = false;
		}
   }
}