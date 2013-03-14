using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Object = Java.Lang.Object;
using iSeconds.Droid;
using iSeconds.Domain;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace CalendarMonthViewAndroid
{
   internal class GridCalendarAdapter : BaseAdapter 
   {
      private Context baseContext;
      private GridView gridViewReference;
      private TimelineViewModel viewModel;

      public GridCalendarAdapter(Context context, TimelineViewModel viewModel, GridView gridViewReference)
      {
         this.gridViewReference = gridViewReference;

         this.baseContext = context;
         this.viewModel = viewModel;

         this.viewModel.PropertyChanged+= this.viewModelChanged;
      }

      private void viewModelChanged(object sender, PropertyChangedEventArgs e)
      {
          this.gridViewReference.InvalidateViews();
      }

      

      public override Object GetItem(int position)
      {
         return null;
      }

      public override long GetItemId(int position)
      {
         return position;
      }

      public override View GetView(int position, View convertView, ViewGroup parent)
      {
         TextView textView;

         if (convertView == null)
         {
            textView = new TextView(baseContext);
            textView.Gravity = GravityFlags.Center;
            textView.SetTextColor(Color.White);
            textView.SetTextSize(Android.Util.ComplexUnitType.Dip, 16);
            //textView.LayoutParameters = new GridView.LayoutParams (85, 85);
         }
         else
         {
            textView = (TextView) convertView;
         }

         if (position < this.Count)
         {
            List<Day> days = viewModel.VisibleDays;
            string dayName = days[position].number.ToString();
            if (position < 7)
               dayName = days[position].shortestDayName + ", " + dayName;

            textView.Text = dayName;
         }

         textView.SetHeight(gridViewReference.Height/6);
         return textView;
      }

      public override int Count
      {
         get { return viewModel.VisibleDays.Count; }
      }
   }

   //[Activity(Label = "CalendarMonthViewAndroid", Icon = "@drawable/icon")]
   //public class Activity1 : Activity
   //{
   //   private CalendarMonth calendar;

   //   protected override void OnCreate(Bundle bundle)
   //   {
   //      base.OnCreate(bundle);
   //      this.SetContentView(Resource.Layout.CalendarMonthView);

   //      calendar= new CalendarMonth(true);

   //      GridView monthView = FindViewById<GridView>(Resource.Id.monthGridView);
   //      monthView.SetNumColumns(7);

   //      GridCalendarAdapter gridCalendarAdapter = new GridCalendarAdapter(this, calendar, monthView);
   //      monthView.Adapter = gridCalendarAdapter;
   //   }
   //}
}

