using Android.App;
using Android.Content;
using iSeconds.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iSeconds.Droid
{
    public class ActivityTracker
    {
        private Activity currentActivity = null;

        public Activity GetCurrentActivity()
        {
            return currentActivity;
        }

        public void SetCurrentActivity(Activity currentActivity)
        {
            this.currentActivity = currentActivity;
        }
    }
}
