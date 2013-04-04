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
using iSeconds.Domain;

namespace iSeconds.Droid
{
    class VideoListAdapter : BaseAdapter
	{
		private DayOptionsViewModel viewModel = null;
		private Context context = null;

		public VideoListAdapter(Context context, DayOptionsViewModel viewModel)
		{
			this.viewModel = viewModel;
			this.context = context;
		}

		public override Java.Lang.Object GetItem (int position)
		{
			return 0;
		}
		public override long GetItemId (int position)
		{
			return 0;
		}


		private bool blocking = false;

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			CheckBox checkBox = new CheckBox(this.context);

			DayOptionsViewModel.VideoItem model = viewModel.Videos[position];
			checkBox.Text = model.Label;
			checkBox.Checked = model.Checked;
			checkBox.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs e) => 
			{
				if (this.blocking) 
					return;

				// se o cara esta checado nao tem como tirar o check..
				if (model.Checked && !e.IsChecked)
				{
					checkBox.Checked = true;
					return;
				}

				if (e.IsChecked)
					this.viewModel.CheckVideoCommand.Execute(position);
			};

			model.OnCheckedChanged += (object sender, GenericEventArgs<bool /*isChecked*/> args) =>
			{
				blocking = true;
				checkBox.Checked = args.Value;
				blocking = false;
			};



			//checkBox.SetOnCheckedChangeListener(new CheckListener(this.viewModel, position));

			return checkBox;
		}

		public override int Count {
			get {
				return viewModel.Videos.Count;
			}
		}
	}

    [Activity(Label = "Day options")]
    public class DayOptionsActivity : Activity
    {
		private ListView listView = null;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            int day = Convert.ToInt32(this.Intent.Extras.GetString("Day"));
            int month = Convert.ToInt32(this.Intent.Extras.GetString("Month"));
            int year = Convert.ToInt32(this.Intent.Extras.GetString("Year"));
            int timelineId = Convert.ToInt32(this.Intent.Extras.GetString("TimelineId"));

            User currentUser = ((ISecondsApplication)this.Application).GetUserService().CurrentUser;
            Timeline timeline = currentUser.GetTimelineById(timelineId);
            DayInfo dayInfo = timeline.GetDayAt(new DateTime(year, month, day));

            this.SetContentView(Resource.Layout.DayOptions);

            DayOptionsViewModel viewModel = new DayOptionsViewModel(dayInfo);

            listView = this.FindViewById<ListView>(Resource.Id.videosList);

			VideoListAdapter adapter = new VideoListAdapter(this, viewModel);
            listView.Adapter = adapter;
        }
	}
}