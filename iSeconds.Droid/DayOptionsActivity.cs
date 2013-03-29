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
    public class DayOptionsViewModel : ViewModel
    {
        private DayInfo model = null;

        private IList<MediaInfo> videos = null;
        public IList<MediaInfo> Videos
        {
            get
            {
                return videos;
            }
            set
            {
                this.SetField(ref videos, value, "Videos");
            }
        }

        public DayOptionsViewModel(DayInfo model)
        {
            this.model = model;

            this.Videos = this.model.GetVideos();
        }
    }

    [Activity(Label = "Day options")]
    public class DayOptionsActivity : Activity
    {
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
            

            ListView listView = this.FindViewById<ListView>(Resource.Id.videosList);
            ArrayAdapter<MediaInfo> adapter = new ArrayAdapter<MediaInfo>(
                this, Resource.Layout.VideoItem, Resource.Id.calendarMonthName, viewModel.Videos);
            listView.Adapter = adapter;
        }
    }
}