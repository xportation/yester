
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
using CalendarMonthViewAndroid;
using System.Windows.Input;
using System.ComponentModel;

namespace iSeconds.Droid
{
    

    public class TimelineView : LinearLayout
    {
        private TimelineViewModel viewModel = null;

        public TimelineView(TimelineViewModel viewModel, Context context)
            : base (context, null)
        {
            this.viewModel = viewModel;

            View.Inflate(context, Resource.Layout.CalendarMonthView, this);

            ImageButton nextMonthButton = this.FindViewById<ImageButton>(Resource.Id.nextMonthButton);
            nextMonthButton.Click += delegate
            {
                this.viewModel.NextMonthCommand.Execute(null);
            };

            ImageButton previousMonthButton = this.FindViewById<ImageButton>(Resource.Id.previousMonthButton);
            previousMonthButton.Click += delegate
            {
                this.viewModel.PreviousMonthCommand.Execute(null);
            };

            Button todayButton = this.FindViewById<Button>(Resource.Id.todayButton);
            todayButton.Click += delegate
            {
                this.viewModel.GoToTodayCommand.Execute(null);
            };

//            calendar = new CalendarMonth(true);

            GridView monthView = FindViewById<GridView>(Resource.Id.monthGridView);
            monthView.SetNumColumns(7);

            GridCalendarAdapter gridCalendarAdapter = new GridCalendarAdapter(this.Context, this.viewModel, monthView);
            monthView.Adapter = gridCalendarAdapter;
        }
    }

    public class HomeView : LinearLayout
    {
        private HomeViewModel viewModel = null;

        public HomeView(HomeViewModel viewModel, Context context)
            : base (context, null)
        {
            this.viewModel = viewModel;

            this.viewModel.PropertyChanged += this.currentTimelineChanged;

            this.viewModel.CurrentTimeline = this.viewModel.CurrentTimeline;
        }

        private void currentTimelineChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentTimeline")
            {
                TimelineViewModel currentTimeline = this.viewModel.CurrentTimeline;
                if (currentTimeline == null)
                {
                    Toast toast = Toast.MakeText(this.Context, "Voce deve criar um timeline", ToastLength.Long);
                    toast.Show();
                }
                else
                {
                    this.AddView(new TimelineView(currentTimeline, this.Context)
                                 , new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent));
                }
            }
        }
    }


	[Activity (Label = "HomeActivity")]			
	public class HomeActivity : Activity
	{
		HomeViewModel viewModel = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			ISecondsApplication application = (ISecondsApplication)this.Application;
			viewModel = new HomeViewModel (application.GetUserService().CurrentUser, application.GetRepository());

            this.SetContentView(Resource.Layout.HomeView);

            LinearLayout layout = this.FindViewById<LinearLayout>(Resource.Id.homeViewLayout);
			layout.AddView(new HomeView(viewModel, this),
			               	new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent));

            //TimelineViewModel timelineViewModel = new TimelineViewModel(application.GetUserService().CurrentUser, application.GetRepository());
            //layout.AddView(new TimelineView(timelineViewModel, this));


            viewModel.NewTimelineCommand.Execute(0);



            //this.StartActivity(typeof(Activity1));



			// Create your application here
		}
	}
}

