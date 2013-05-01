
using Android.Widget;
using Android.Views;
using iSeconds.Domain;
using System.Collections.Generic;
using System.ComponentModel;
using Android.Content;

namespace iSeconds.Droid
{
   public class TimelineView : LinearLayout, View.IOnCreateContextMenuListener, IMenuItemOnMenuItemClickListener
   {
      private TimelineViewModel viewModel = null;
   
      public TimelineView (TimelineViewModel viewModel, Context context)
         : base(context, null)
      {
         this.viewModel = viewModel;
      
         Inflate (context, Resource.Layout.CalendarMonthViewLayout, this);
      
         TextView monthLabel = this.FindViewById<TextView> (Resource.Id.calendarMonthName);
         TextViewUtil.ChangeFontForMonthTitle(monthLabel, context, 20f);
         monthLabel.Text = this.viewModel.CurrentMonthTitle;
      
         CalendarMonthViewWeekNames monthWeekNames =
         FindViewById<CalendarMonthViewWeekNames> (Resource.Id.calendarWeekDays);
         List<DayViewModel> weekDays = new List<DayViewModel> (viewModel.VisibleDays.GetRange (0, 7));
         monthWeekNames.WeekDays = weekDays;
      
         CalendarMonthView monthView = FindViewById<CalendarMonthView> (Resource.Id.calendarView);
         monthView.ViewedDays = viewModel.VisibleDays;
         monthView.ViewModel = viewModel;
      
         this.viewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
         {
            if (e.PropertyName == "CurrentMonthTitle") {
               monthLabel.Text = this.viewModel.CurrentMonthTitle;
            }
         
            if (e.PropertyName == "VisibleDays") {
               monthView.ViewedDays = viewModel.VisibleDays;
            
               foreach (DayViewModel day in viewModel.VisibleDays) {
                  day.DayOptionsRequest.Raised += (object s, GenericEventArgs<DayViewModel.DayOptionsList> args) => 
                  {
                     optionsList = args.Value;
                     ShowContextMenu ();
                  };
               }              
            }
         };
      
         SetOnCreateContextMenuListener (this);
      }
   
      public override void OnWindowFocusChanged (bool hasFocus)
      {
         base.OnWindowFocusChanged (hasFocus);
         // precisei fazer pois depois do menu de contexto aberto, se apertarmos o back button a seleção não é removida
         if (hasFocus)
            this.Invalidate ();
      }
   
      private DayViewModel.DayOptionsList optionsList;
   
      public bool OnMenuItemClick (IMenuItem item)
      {
         optionsList.DayEntryClicked.Execute (item.ItemId);
         return true;
      }
   
      public void OnCreateContextMenu (IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
      {
         for (int i = 0; i < optionsList.OptionsEntries.Count; i++) {
            OptionsList.OptionsEntry entry = optionsList.OptionsEntries [i];
            IMenuItem menuItem = menu.Add (0, i, i, entry.Name);
            menuItem.SetOnMenuItemClickListener (this);
         }
      }
   
   }


}