using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using iSeconds.Domain;
using System.ComponentModel;

namespace iSeconds.Droid
{
   
   public class TimelineView : LinearLayout
   {
      private TimelineViewModel viewModel = null;

      public TimelineView(TimelineViewModel viewModel, Context context)
         : base(context, null)
      {
         this.viewModel = viewModel;

         Inflate(context, Resource.Layout.CalendarMonthViewLayout, this);

         TextView monthLabel = this.FindViewById<TextView>(Resource.Id.calendarMonthName);
         monthLabel.Text = this.viewModel.CurrentMonthTitle;

         CalendarMonthViewWeekNames monthWeekNames =
            FindViewById<CalendarMonthViewWeekNames>(Resource.Id.calendarWeekDays);
         List<DayViewModel> weekDays = new List<DayViewModel>(viewModel.VisibleDays.GetRange(0, 7));
         monthWeekNames.WeekDays = weekDays;

         CalendarMonthView monthView = FindViewById<CalendarMonthView>(Resource.Id.calendarView);
         monthView.ViewedDays= viewModel.VisibleDays;
         monthView.ViewModel = viewModel;

         this.viewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
               if (e.PropertyName == "CurrentMonthTitle")
               {
                  monthLabel.Text = this.viewModel.CurrentMonthTitle;
               }

               if (e.PropertyName == "VisibleDays")
               {
                  monthView.SetViewedDaysAnimated(viewModel.VisibleDays);
               }
            };
      }

   }

   public class HomeView : LinearLayout
   {
      private HomeViewModel viewModel = null;

      public HomeView(HomeViewModel viewModel, Context context)
         : base(context, null)
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
                            ,
                            new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent,
                                                       ViewGroup.LayoutParams.FillParent));
            }
         }
      }
   }


   [Activity(Label = "HomeActivity")]
   public class HomeActivity : ISecondsActivity
   {
      private HomeViewModel viewModel = null;
      private GestureDetector gestureDetector;

      protected override void OnCreate(Bundle bundle)
      {
         base.OnCreate(bundle);

         ISecondsApplication application = (ISecondsApplication) this.Application;
         viewModel = new HomeViewModel(application.GetUserService().CurrentUser, application.GetRepository(), application.GetMediaService());

         this.RequestWindowFeature(WindowFeatures.NoTitle);
         this.SetContentView(Resource.Layout.HomeView);

         LinearLayout layout = this.FindViewById<LinearLayout>(Resource.Id.homeViewLayout);
         layout.AddView(new HomeView(viewModel, this),
                        new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent));

          // temporariamente para teste.. se nao existir nenhum timeline irá criar, se já existir irá sempre pegar a primeira
         IList<Timeline> timelines = application.GetRepository().GetUserTimelines(application.GetUserService().CurrentUser.Id);
         if (timelines.Count == 0)
             viewModel.NewTimelineCommand.Execute(null);
         else 
             viewModel.LoadTimelineCommand.Execute(timelines[0].Id);

      }

   }
}
