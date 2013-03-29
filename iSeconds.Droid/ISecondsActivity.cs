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
    public class ISecondsActivity : Activity
    {
        protected ActivityTracker activityTracker = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            activityTracker = ((ISecondsApplication)this.Application).GetActivityTracker();
        }

        protected override void OnResume()
        {
            base.OnResume();
            activityTracker.SetCurrentActivity(this);
        }

        protected override void OnPause()
        {
            clearReferences();
            base.OnPause();
        }

        protected override void OnDestroy()
        {
            clearReferences();
            base.OnDestroy();
        }

        private void clearReferences()
        {
            Activity currActivity = activityTracker.GetCurrentActivity();
            if (currActivity != null && currActivity.Equals(this))
                activityTracker.SetCurrentActivity(null);
        }
    }
}