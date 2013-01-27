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

namespace CalendarControl
{
	class CalendarMonthView : LinearLayout
	{
		// CONSTS
		// 
		private const int textViewCalendarCaptionId = 101;
		
		// CONTROLS
		// 
		// Header controls
		private Button btnPrevMonth;
		private TextView HeaderTextView;
		private Button btnNextMonth;
		
		// GridView and adapter
		private GridView calendarGridView;
		private CalendarViewAdapter adapter;
		
		// HANDLERS
		// 
		// Function to check that date is available
		public Func<DateTime, bool> IsDateAvailable
		{ set { adapter.IsDateAvailable = value; } }
		
		// Events
		public Action<DateTime> OnDateSelect = null;
		public Action<DateTime> OnMonthChanged = null;
		public bool GoCurrentMonthOnTitleClick = true;

		// VIEW OPTIONS
		// 
		// Days
		public bool HighlightWeekend {
			get { return adapter.HighlightWeekend; }
			set { 
				adapter.HighlightWeekend = value;
				UpdateCalendar(adapter.GetDate());
			}
		}
		
		public List<DayOfWeek> WeekendDays {
			get { return adapter.WeekendDays; }
			set {
				adapter.WeekendDays = value;
				UpdateCalendar(adapter.GetDate());
			}
		}
		
		public CalendarViewAdapter.FirstDayOfWeekMode FirstDayOfWeek {
			get { return adapter.FirstDayOfWeek; }
			set { adapter.FirstDayOfWeek = value; }
		}
		
		// Colors
		private Color _HeaderTextColor = Color.White;
		public Color HeaderTextColor {
		    get { return _HeaderTextColor; }
		    set {
				_HeaderTextColor = value;
				if (HeaderTextView != null)
					HeaderTextView.SetTextColor(value);
			}
		}
		
		public Color WeekendColor {
		    get { return adapter.WeekendColor; }
		    set { adapter.WeekendColor = value;	}
		}

		public CalendarMonthView(Context context, Android.Util.IAttributeSet attr)
			: base(context)
		{
			Orientation = Orientation.Vertical;
			//LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent);
			//SetBackgroundColor(Color.MediumVioletRed);

			
			adapter = new CalendarViewAdapter(context, DateTime.Now);
			calendarGridView = new GridView(context);
			calendarGridView.Adapter = adapter;
			calendarGridView.SetNumColumns(7);
			calendarGridView.LayoutParameters = 
				new LinearLayout.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.MatchParent, 1f);
			AddView(calendarGridView);
			
			// Handlers
			adapter.OnItemClicked = OnDateSelectInternal;
			
			AddCalendarHeader(context);
			UpdateCalendar(adapter.GetDate());
		}

		public CalendarViewAdapter Adapter {
			get {
				return adapter;
			}
		}
		
		private void OnDateSelectInternal(DateTime date)
		{
			if (OnDateSelect != null)
				OnDateSelect(date);
		}
		
		private void AddCalendarHeader(Context context)
		{
			// Parts of header
			// Button "Previous month"
			btnPrevMonth = new Button(context);
			btnPrevMonth.Text = "<";
			btnPrevMonth.Id = 100;
	        btnPrevMonth.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent, 1f);
			btnPrevMonth.Click += (sender, e) => {
				// Attempt #1
				//setDate(adapter.getDate().AddMonths(-1));
				
				// Attempt #2
				//DateTime newDate = adapter.getDate().AddMonths(-1);
				//
				//adapter.setDate(newDate);
				//updateCalendar(newDate);
				
				//if (OnMonthChanged != null)
				//	OnMonthChanged(newDate);
				
				// Attempt #3
				DateTime newDate = adapter.GetDate().AddMonths(-1);
				SetDate(newDate, false);
			};
			adapter.OnPrevMonthItemClick = delegate(DateTime obj) {
				SetDate(adapter.GetDate().AddMonths(-1));
			};
			
			// Header text. Example: "March 2012"
		    TextView textViewCaption = new TextView(context);
	        textViewCaption.Text = DateTime.Now.ToShortDateString();
			textViewCaption.SetTextColor(HeaderTextColor);
			textViewCaption.SetTextSize(Android.Util.ComplexUnitType.Dip, 28);
			textViewCaption.Gravity = GravityFlags.Center;
	        textViewCaption.Id = textViewCalendarCaptionId;
			// MatchParent, чтобы высота была как у кнопок, а, следовательно, текст по высоте точно выровнен
	        textViewCaption.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, /*ViewGroup.LayoutParams.WrapContent*/ ViewGroup.LayoutParams.MatchParent, 4f);
			HeaderTextView = textViewCaption;
			
			textViewCaption.Click += delegate {
				// Change month to current month
				if (GoCurrentMonthOnTitleClick){
					DateTime curDate = adapter.GetDate();
					DateTime newDate = new DateTime(curDate.Year, DateTime.Now.Month, curDate.Day); 
					
					/*adapter.setDate(newDate);
					updateCalendar(newDate);
					
					if (OnMonthChanged != null)
						OnMonthChanged(newDate);*/
					SetDate(newDate, true);
				}
			};
			
			// Button "Next month"
			btnNextMonth = new Button(context);
			btnNextMonth.Text = ">";
			btnNextMonth.Id = 102;
	        btnNextMonth.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent, 1f);
			btnNextMonth.Click += (sender, e) => {
				// Attempt #1
				//setDate(adapter.getDate().AddMonths(1));
				
				// Attempt #2
				//DateTime newDate = adapter.getDate().AddMonths(1);
				//
				//adapter.setDate(newDate);
				//updateCalendar(newDate);
				//
				//if (OnMonthChanged != null)
				//	OnMonthChanged(newDate);

				// Attempt #3
				DateTime newDate = adapter.GetDate().AddMonths(1);
				SetDate(newDate, false);
			};
			adapter.OnNextMonthItemClick = delegate(DateTime obj) {
				SetDate(adapter.GetDate().AddMonths(1));
			};
			
			// Combine title elements
			//
			LinearLayout layoutCaption = new LinearLayout(context);
			layoutCaption.Orientation = Orientation.Horizontal;
	        layoutCaption.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
		
			layoutCaption.AddView(btnPrevMonth);
			layoutCaption.AddView(textViewCaption);
			layoutCaption.AddView(btnNextMonth);
			
			// Add header layout to view
			this.AddView(layoutCaption, 0);
		}		
		
		public void SetDate(DateTime dateTime, bool callOnDateSelect = true)
		{
			// MCS on 2012.03.24:
			// Plan:
			// 1. Set new date in adapter and repaint calendar;
			// 2. Collect garbage (clear memory from old cells);
			// 3. Call event handlers
			
			// 1. 
			adapter.SetDate(dateTime);
			UpdateCalendar(dateTime);
			
			// 2. 
			GC.Collect();

			// 3.
			if (OnDateSelect != null)
				OnDateSelect(dateTime);

			if (OnMonthChanged != null)
				OnMonthChanged(dateTime);
		}
		
		public DateTime GetDate()
		{ return adapter.GetDate(); }

		private void UpdateCalendar(DateTime date)
		{
			TextView textViewCaption = FindViewById<TextView>(textViewCalendarCaptionId);
	        textViewCaption.Text = date.ToString ("MMMM yyyy");
			
			calendarGridView.Adapter = adapter;
			calendarGridView.InvalidateViews();
		}		

		public void InvalidateCalendar ()
		{
			calendarGridView.InvalidateViews();
		}
	}
}

