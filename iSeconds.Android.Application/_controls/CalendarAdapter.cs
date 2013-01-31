// Copyright 2012 Maxim Korobov @ Favorit Systems
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//		http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Globalization;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

/// <summary>
/// Colors in Android
/// </summary>
using Android.Graphics;
using Android.Util;

namespace CalendarControl
{
	class CalendarViewAdapter: BaseAdapter
	{
		public delegate TextView CustomizeItemDelegate(TextView textView, DateTime date);
		public CustomizeItemDelegate CustomizeItem;

		private Context baseContext;

		// Date and month info
		private DateTime baseDate;
		private DateTime FirstDateOfMonth;
		private int FirstDayOfWeekNumber;
		private int DaysInMonth;
		
		// Highlights
		public bool HighlightWeekend = true;
		//public bool HighlightDayOffInHeader = false;
		public List<DayOfWeek> WeekendDays = new List<DayOfWeek>() {DayOfWeek.Saturday, DayOfWeek.Sunday};
		
		// Set date background color based on IsDateAvailable result
		public Func<DateTime, bool> IsDateAvailable;
		//public Func<DateTime, bool> IsDayMarkedDelegate;

		// Events
		public Action<DateTime> OnItemClicked = null;
		
		// Month changed
		public Action<DateTime> OnPrevMonthItemClick = null;
		public Action<DateTime> OnNextMonthItemClick = null;
		
		// Colors
		public Color AvailableDateColor = Color.Gray;
		public Color FocusedDateColor = Color.White;
		public Color WeekendColor = Color.Red;
		
		// Selected cell. Need to save to correctly repaint it when other cell will be selected
		public TextView focusedItem = null;
		
		// First day of week routines
		public enum FirstDayOfWeekMode {Sunday = DayOfWeek.Sunday, Monday = DayOfWeek.Monday, Autodetect = -1};
		public FirstDayOfWeekMode FirstDayOfWeek = FirstDayOfWeekMode.Autodetect;
		private DayOfWeek dayOfWeek;

		private Android.Views.IWindowManager windowManager;

		private int HeaderHeight;
		
	    public CalendarViewAdapter(Context context, DateTime date, int HeaderHeight) 
        {
			this.HeaderHeight = HeaderHeight;
			baseContext = context;
			SetDate(date);

			windowManager = this.baseContext.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
		}
		
		public void SetDate(DateTime date)
		{
			baseDate = date.Date;

			// First day of month
			FirstDateOfMonth = new DateTime(date.Year, date.Month, 1);
			if (FirstDayOfWeek == FirstDayOfWeekMode.Autodetect)
				dayOfWeek = GetFirstDayOfWeek(DateTime.Now).DayOfWeek;
			else
				dayOfWeek = (DayOfWeek)FirstDayOfWeek;
			
			if (dayOfWeek == DayOfWeek.Sunday)
				FirstDayOfWeekNumber = (int)FirstDateOfMonth.DayOfWeek;
			else {
				if (FirstDateOfMonth.DayOfWeek == DayOfWeek.Sunday)
					FirstDayOfWeekNumber = 6;
				else
					FirstDayOfWeekNumber = (int)FirstDateOfMonth.DayOfWeek - 1;
			}
			
			// Calculate once "Days in month"
			DaysInMonth = DateTime.DaysInMonth(baseDate.Year, baseDate.Month);
		}
		
		public string GetDayOfWeekName(int dateNumber)
		{
			if (dayOfWeek == DayOfWeek.Sunday)
				return DateTimeFormatInfo.CurrentInfo.AbbreviatedDayNames[dateNumber];
			else {
				DayOfWeek tmpFirstDayOfWeek = GetFirstDayOfWeek(DateTime.Now).DayOfWeek;
				int daysCount = DateTimeFormatInfo.CurrentInfo.AbbreviatedDayNames.Count();
				if ((int)tmpFirstDayOfWeek + dateNumber < daysCount)
					return DateTimeFormatInfo.CurrentInfo.AbbreviatedDayNames[(int)tmpFirstDayOfWeek + dateNumber];
				else
					return DateTimeFormatInfo.CurrentInfo.AbbreviatedDayNames[(int)tmpFirstDayOfWeek + dateNumber - daysCount];
			}
		}
		
		public DateTime GetDate()
		{
			return baseDate;
		}
		
		// Date time culture info (http://joelabrahamsson.com/entry/getting-the-first-date-in-a-week-with-c-sharp)
		/// <summary>
	    /// Returns the first day of the week that the specified
	    /// date is in using the current culture. 
	    /// </summary>
	    public static DateTime GetFirstDayOfWeek(DateTime dayInWeek)
	    {
	        CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
	        return GetFirstDayOfWeek(dayInWeek, defaultCultureInfo);
	    }
	
	    /// <summary>
	    /// Returns the first day of the week that the specified date 
	    /// is in. 
	    /// </summary>
	    public static DateTime GetFirstDayOfWeek(DateTime dayInWeek, CultureInfo cultureInfo)
	    {
	        DayOfWeek firstDay = cultureInfo.DateTimeFormat.FirstDayOfWeek;
	        DateTime firstDayInWeek = dayInWeek.Date;
	        while (firstDayInWeek.DayOfWeek != firstDay)
	            firstDayInWeek = firstDayInWeek.AddDays(-1);
	
	        return firstDayInWeek;
	    }		
		
		//
        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return position;
        }
		
		private bool IsDateAvailableOrNull(DateTime date)
		{
			if (IsDateAvailable != null) 
			    return (IsDateAvailable(date));
			else
				return true;
		}

		private int getDisplayHeight ()
		{
			// codigo para pegar a altura do display removendo a altura do notification bar e do title bar
			// http://stackoverflow.com/questions/3600713/size-of-android-notification-bar-and-title-bar
			DisplayMetrics metrics = new DisplayMetrics ();
			windowManager.DefaultDisplay.GetMetrics (metrics);
			int myHeight = 0;

			// multiplicado por 2 pois e o title + notification
			switch (metrics.DensityDpi) {
			case DisplayMetricsDensity.High:
				return windowManager.DefaultDisplay.Height - (48 * 2); 
			case DisplayMetricsDensity.Medium:
				return windowManager.DefaultDisplay.Height - (32 * 2);
			case DisplayMetricsDensity.Low:
				return windowManager.DefaultDisplay.Height - (24 * 2);
			default:
				return myHeight;
			}

		}

		private int calculateRowHeight ()
		{
			// the row height will be the display height minus the header height, divided by the row count (Count/NUM_OF_COLLUMNS)
			return (getDisplayHeight() - HeaderHeight) / (Count/7);
		}

        public override View GetView (int position, View convertView, ViewGroup parent)
		{
			TextView textView;

			if (convertView == null) {  // if it's not recycled, initialize some attributes
				textView = new TextView (baseContext);

				// to expand the grid, we measure 
				textView.SetHeight(calculateRowHeight());

				textView.Gravity = GravityFlags.Center;
				textView.SetTextColor (Color.White);
				textView.SetTextSize (Android.Util.ComplexUnitType.Dip, 20);
				textView.Clickable = true; // disable default orangle highlight when cell clicked
				
				// Calendar header (Sun, Mon and so on)
				if (position < 7) {
					textView.Text = GetDayOfWeekName (position);
					/*
					if (HighlightDayOffInHeader) {
						if ((position == 5) || (position == 6))
							textView.SetTextColor(WeekendColor);
					}*/
				} else {

					DateTime dateInMonth = DateTime.Now;
					int dayInCurrentMonth = GetDayInCurrentMonth (position);

					// verify if position is in current month
					if (IsCurrentMonthPosition (position)) {
						textView.Text = (dayInCurrentMonth).ToString ();
						textView.Tag = position; // Not the best solution to store position, but the fastest
					
						// Highlight available days
						DateTime date = new DateTime (baseDate.Year, baseDate.Month, dayInCurrentMonth);
						
						// If IsDateAvailable func is not null, only date with true result of that function
						// will be available for selection (and HandleTextViewClick will be called)
						if (IsDateAvailableOrNull (date)) {
							textView.SetBackgroundColor (AvailableDateColor);
							textView.SetTextColor (Color.Black);
							// Highlight focused date
							if (date == baseDate) {
								textView.SetBackgroundColor (FocusedDateColor);
								focusedItem = textView;
							}
							textView.Click += HandleTextViewClick;					
						}

						dateInMonth = date;
						
						// Highligh day off (only for date in current month)
						if ((HighlightWeekend) && 
						//((dateInMonth.DayOfWeek == DayOfWeek.Saturday) || (dateInMonth.DayOfWeek == DayOfWeek.Sunday)))
							(WeekendDays.Contains (dateInMonth.DayOfWeek)))
							textView.SetTextColor (WeekendColor);						
					} else {

						// item is not from current month
						int positionNextMonth = PositionInNextMonth (position);
						if (positionNextMonth > 0) { 
							// is next month...
							textView.SetTextColor (Color.DarkGray);
							DateTime dayInNextMonth = new DateTime (baseDate.Year, baseDate.Month, positionNextMonth).AddMonths (1);
							textView.Text = dayInNextMonth.Day.ToString ();//positionNextMonth.ToString();
							//
							dateInMonth = dayInNextMonth;
							textView.Click += delegate {
								if (OnNextMonthItemClick != null)
									OnNextMonthItemClick (dateInMonth);
							};

							
						} else {
							// is previou month...
							int positionPrevMonth = PositionInPrevMonth (position);
							if (positionPrevMonth < 0) {
								textView.SetTextColor (Color.DarkGray);
								DateTime dayInPrevMonth = new DateTime (baseDate.Year, baseDate.Month, 1).AddDays (positionPrevMonth);
								textView.Text = dayInPrevMonth.Day.ToString ();//FirstDateOfMonth.AddDays(positionPrevMonth).Day.ToString();
								
								dateInMonth = dayInPrevMonth;
								textView.Click += delegate {
									if (OnPrevMonthItemClick != null)
										OnPrevMonthItemClick (dateInMonth);
								};
							}
						}
					}

					textView = CustomizeItem (textView, dateInMonth);


					// Highlight day off in Previous and next months too
					//if ((HighlightDayOff) && 
					//   ((dateInMonth.DayOfWeek == DayOfWeek.Saturday) || (dateInMonth.DayOfWeek == DayOfWeek.Sunday)))
					//	textView.SetTextColor(Color.Red);
				}
			} else {
				textView = (TextView)convertView;
				textView = CustomizeItem (textView, GetDateByPosition(position));

			}
			//Console.WriteLine("Position: {0}", position);

			textView.Tag = position;
			
	        return textView;
        }



		int GetDayInCurrentMonth(int position)
		{
			// - 7 cause first 7 cells are header with days short names
			return position - FirstDayOfWeekNumber + 1 - 7;
		}
		
		bool IsCurrentMonthPosition(int position)
		{
			return ((PositionInPrevMonth(position) >= 0) && (PositionInNextMonth(position) < 1));
		}

		int PositionInPrevMonth(int position)
		{
			int dayInCurrentMonth = GetDayInCurrentMonth(position) - 1;
			return dayInCurrentMonth;
		}
		
		int PositionInNextMonth(int position)
		{
			int dayInCurrentMonth = GetDayInCurrentMonth(position);
			return (dayInCurrentMonth - DaysInMonth);
		}
		
        void HandleTextViewClick (object sender, EventArgs e)
        {
			// base.onclick ?
			
			// Warning! Should be called BEFORE setDate()
			if (focusedItem != null)
				focusedItem.SetBackgroundColor(AvailableDateColor);
			//
			
			TextView clickedItem = (sender as TextView);
			int position = (int)clickedItem.Tag;
			int dayInCurrentMonth = GetDayInCurrentMonth(position);
			if (IsCurrentMonthPosition(position)) {
				DateTime selectedDate = new DateTime(baseDate.Year, baseDate.Month, dayInCurrentMonth);
				
				SetDate(selectedDate);
				clickedItem.SetBackgroundColor(FocusedDateColor);
				if (OnItemClicked != null)
					OnItemClicked(selectedDate);
			}
			focusedItem = clickedItem;
			
        	//Console.WriteLine((sender as TextView).Text);
        }

		public DateTime GetDateByPosition (int position)
		{
			int dayInCurrentMonth = GetDayInCurrentMonth(position);
			int positionNextMonth = this.PositionInNextMonth(position);
			int positionPreviousMonth = this.PositionInPrevMonth(position);

			if (IsCurrentMonthPosition(position))
				return new DateTime(baseDate.Year, baseDate.Month, dayInCurrentMonth);
			else if (positionNextMonth > 0)
				return new DateTime(baseDate.Year, baseDate.Month, positionNextMonth).AddMonths(1);
			else if (positionPreviousMonth < 0)
				return new DateTime(baseDate.Year, baseDate.Month, 1).AddDays(positionPreviousMonth);

			throw new Exception("cant reach");
		}

        public override int Count
        {
            get {
				// +1 cause FirstDayOfWeek counts from 0, but number of cells from 1
				// - 7 cause first 7 cells are header with days short names
				int daysCount = DaysInMonth + FirstDayOfWeekNumber + 1 + 7;
				while (daysCount % 7 != 0)
					daysCount++;
				return daysCount;
			}
        }
	}
}

 