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
   class CalendarGestureListener : GestureDetector.SimpleOnGestureListener
   {
      private HomeViewModel viewModel;

      public CalendarGestureListener(HomeViewModel viewModel)
      {
         this.viewModel = viewModel;
      }

      public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
      {
         const float tolerance = 15;
         float angle = getAngleInDegrees(e1.GetX(), e1.GetY(), e2.GetX(), e2.GetY());

         if ((angle < 90 + tolerance && angle > 90 - tolerance) || (angle < 270 + tolerance && angle > 270 - tolerance))
         {
            if(e1.GetY() > e2.GetY())
               viewModel.CurrentTimeline.NextMonthCommand.Execute(null);
            else
               viewModel.CurrentTimeline.PreviousMonthCommand.Execute(null);
            
            return true;
         }

         return false;
      }

      private float getAngleInDegrees(float x1, float y1, float x2, float y2)
      {
         float dx, dy, angle;
         dx = x2 - x1;
         dy = y2 - y1;
         if ((Math.Abs(dx) + Math.Abs(dy)) < 0.00000001)
            return 0;

         angle = (float)Math.Atan2(dy, dx);
         if (angle < 0)
            angle = (float)(2 * Math.PI + angle);

         return (float)(angle * 180 / Math.PI);
      }
   }

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
         List<Day> weekDays = new List<Day>(viewModel.VisibleDays.GetRange(0, 7));
         monthWeekNames.WeekDays = weekDays;

         CalendarMonthView monthView = FindViewById<CalendarMonthView>(Resource.Id.calendarView);
         monthView.ViewedDays = viewModel.VisibleDays;

         this.viewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
               if (e.PropertyName == "CurrentMonthTitle")
               {
                  monthLabel.Text = this.viewModel.CurrentMonthTitle;
               }

               if (e.PropertyName == "CurrentDate")
               {
                  monthView.ViewedDays = viewModel.VisibleDays;         
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
   public class HomeActivity : Activity
   {
      private HomeViewModel viewModel = null;
      private GestureDetector gestureDetector;

      protected override void OnCreate(Bundle bundle)
      {
         base.OnCreate(bundle);

         ISecondsApplication application = (ISecondsApplication) this.Application;
         viewModel = new HomeViewModel(application.GetUserService().CurrentUser, application.GetRepository());

         this.RequestWindowFeature(WindowFeatures.NoTitle);
         this.SetContentView(Resource.Layout.HomeView);

         LinearLayout layout = this.FindViewById<LinearLayout>(Resource.Id.homeViewLayout);
         layout.AddView(new HomeView(viewModel, this),
                        new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent));

         //TimelineViewModel timelineViewModel = new TimelineViewModel(application.GetUserService().CurrentUser, application.GetRepository());
         //layout.AddView(new TimelineView(timelineViewModel, this));

         viewModel.NewTimelineCommand.Execute(0);

         gestureDetector= new GestureDetector(new CalendarGestureListener(viewModel));
         //this.StartActivity(typeof(Activity1));
         // Create your application here
      }

      public override bool OnTouchEvent(MotionEvent e)
      {
         gestureDetector.OnTouchEvent(e);
         return base.OnTouchEvent(e);
      }
   }
}