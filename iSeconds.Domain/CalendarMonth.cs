using System;
using System.Collections.Generic;
using System.Globalization;
using iSeconds.Domain;

namespace iSeconds.Domain
{
   public struct Day 
   {
      public int number;
      public string dayName;
      public string shortestDayName;
      public bool inCurrentMonth;
      public bool isWeekend;
      public bool isToday;
      public DateTime day;
   }

   public class CalendarMonth
   {
      private const int WeekDaysCount = 7;
      private const int FixedRowsCount = 6;

      private bool usingFixedRows;
      private DateTime currentDate;

      private List<Day> currentDays= new List<Day>();

      public CalendarMonth(bool usingFixedRows, DateTime currentDate)
      {
         this.currentDate = currentDate;
         this.usingFixedRows = usingFixedRows;
      }

      public DateTime CurrentDate
      {
         get { return currentDate; }
         set { 
            currentDate = value; 
            currentDays.Clear();
         }
      }
      
      public List<Day> GetViewedDays()
      {
         if (currentDays.Count > 0)
            return currentDays;

         int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
         DateTime firstDateOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
         int dayOfWeekIndex = getDayOfWeekIndex(firstDateOfMonth);
         
         if (dayOfWeekIndex == 0 && usingFixedRows)
            dayOfWeekIndex += WeekDaysCount;

         DateTime startDate = firstDateOfMonth.AddDays(-dayOfWeekIndex);
         int totalDays = daysInMonth + dayOfWeekIndex;

         if (usingFixedRows)
            totalDays = WeekDaysCount * FixedRowsCount;
         else
         {
            int temporaryColumnIndex = totalDays % WeekDaysCount;
            totalDays += (temporaryColumnIndex > 0) ? WeekDaysCount - temporaryColumnIndex : 0;
         }

	      for (int i = 0; i < totalDays; i++)
	      {
            currentDays.Add(new Day
               {
                  number = startDate.Day, 
                  inCurrentMonth= startDate.Month == currentDate.Month,
                  dayName = DateTimeFormatInfo.CurrentInfo.GetDayName(startDate.DayOfWeek),
                  shortestDayName = DateTimeFormatInfo.CurrentInfo.GetShortestDayName(startDate.DayOfWeek),
                  isWeekend = startDate.DayOfWeek == DayOfWeek.Saturday || startDate.DayOfWeek == DayOfWeek.Sunday,
                  isToday = startDate.Date == DateTime.Now.Date,
                  day = startDate
               } );
	         startDate = startDate.AddDays(1);
	      }

	      return currentDays;
      }

	   private int getDayOfWeekIndex(DateTime day)
	   {
	      Dictionary<DayOfWeek, int> dayIndex = new Dictionary<DayOfWeek, int>()
	         {
	            { DayOfWeek.Sunday, 0 }, { DayOfWeek.Monday, 1 }, { DayOfWeek.Tuesday, 2 },
	            { DayOfWeek.Wednesday, 3 }, { DayOfWeek.Thursday, 4 }, { DayOfWeek.Friday, 5 }, { DayOfWeek.Saturday, 6 }
	         };

	      int index = dayIndex[day.DayOfWeek];
	      if (DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek == DayOfWeek.Monday)
	         index = (index > 0) ? --index : 6;

			return index;
	   }
   }
}
