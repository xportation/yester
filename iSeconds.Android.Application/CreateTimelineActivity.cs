
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
	[Activity (Label = "Create Timeline", Theme = "@android:style/Theme.Dialog")]
	public class CreateTimelineActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.CreateTimeline);
			
			Button create = FindViewById<Button> (Resource.Id.create_timeline_button);
			EditText editText = FindViewById<EditText> (Resource.Id.timeline_name_edit);
			Button cancel = FindViewById<Button> (Resource.Id.cancel_timeline_creation_button);

			create.Click += delegate {
				this.Intent.PutExtra(ISecondsConstants.TIMELINE_NAME_EXTRA, editText.Text);
				this.SetResult(Result.Ok, this.Intent);
				this.Finish();
			};

			cancel.Click+= delegate {
				this.SetResult(Result.Canceled, this.Intent);
				this.Finish();
			};
		}
	}
}

