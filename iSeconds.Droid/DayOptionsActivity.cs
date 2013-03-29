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
    [Activity(Label = "Day options")]
    public class DayOptionsActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            string id = this.Intent.Extras.GetString("DayId");

            Toast.MakeText(this, id, ToastLength.Long).Show();
        }
    }
}