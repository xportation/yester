using CalendarMonthViewAndroid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;

namespace iSeconds.Domain
{
	public class TimelineViewModel : ViewModel
	{
		private Timeline timeline = null;
		private IRepository repository = null;
		
		public TimelineViewModel(Timeline timeline, IRepository repository)
		{
			this.timeline = timeline;
			this.repository = repository;

            CalendarMode = VisualizationMode.MONTH;
			
			Days = this.repository.GetDaysInMonth (this.timeline.Id, DateTime.Today.Month, DateTime.Today.Year);

            this.CurrentDate = DateTime.Today;
		}

        private DateTime currentDate;
        public DateTime CurrentDate
        {
            get { return currentDate; }
            set {

                // se mudou o mes ou o ano devemos o title do mes
                if (currentDate.Month != value.Month || currentDate.Year != value.Year)
                {
                    CurrentMonthTitle = this.prepareCurrentMonthTitle(value.Month, value.Year);
                }

                CalendarMonth calendarMonth = new CalendarMonth(true, value);
                VisibleDays = calendarMonth.GetViewedDays();

                this.SetField(ref currentDate, value, "CurrentDate"); 
            }
        }

        private string prepareCurrentMonthTitle(int month, int year)
        {
            string strMonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            return strMonthName + ", " + year.ToString();
        }

        public enum VisualizationMode { MONTH, WEEK }

        private VisualizationMode calendarMode;
        public VisualizationMode CalendarMode
        {
            get { return calendarMode; }
            set
            {
                this.SetField(ref calendarMode, value, "CalendarMode");
            }
        }


        List<Day> visibleDays = new List<Day>();
        public List<Day> VisibleDays
        {
            get { return visibleDays; }
            set {
                this.SetField(ref visibleDays, value, "VisibleDays");
            }

        }

        public ICommand AddVideoAt
        {
            get {
                return new Command(null);
                //return new Command((DateTime date, string videoPath) => {

                //    //this.repository.SaveDayInfo(new DayInfo(date, this.timeline.Id));
					
                //});
            }
        }

        public ICommand NextMonthCommand
        {
            get
            {
                return new Command(delegate
                {
                    this.CurrentDate = this.CurrentDate.AddMonths(1);
                });
            }
        }

        public ICommand PreviousMonthCommand
        {
            get
            {
                return new Command(delegate
                {
                    this.CurrentDate = this.CurrentDate.AddMonths(-1);
                });
            }
        }

        public ICommand GoToTodayCommand
        {
            get
            {
                return new Command(delegate
                {
                    this.CurrentDate = DateTime.Today;
                });
            }
        }
		
		public IList<DayInfo> Days { get; set; }


        private string currentMonthTitle;
        public string CurrentMonthTitle 
        { 
            get 
            {
                return currentMonthTitle;
            }
            set 
            {
                this.SetField(ref currentMonthTitle, value, "CurrentMonthTitle");
            }
        }
    }
}

