
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

namespace iSeconds.Android.Application
{
	[Activity (Label = "Create Timeline")]
	public class CreateTimelineActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.CreateTimeline);
			
			Button button = FindViewById<Button> (Resource.Id.create_timeline_button);
			EditText editText = FindViewById<EditText> (Resource.Id.timeline_name_edit);

			button.Click += delegate {
				this.Intent.PutExtra("TIMELINE_NAME_EXTRA", editText.Text);
				this.SetResult(Result.Ok, this.Intent);
				this.Finish();
			};
		}
	}
}

