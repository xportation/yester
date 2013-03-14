using CalendarMonthViewAndroid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

            PropertyChanged += this.CurrentDateChanged;

            this.CurrentDate = DateTime.Today;
		}

        private void CurrentDateChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentDate")
            {
                CalendarMonth calendarMonth = new CalendarMonth(true, (DateTime)this.GetType().GetProperty(e.PropertyName).GetValue(this, null));
                VisibleDays = calendarMonth.GetViewedDays();
            }
        }

        

        private DateTime currentDate;
        public DateTime CurrentDate
        {
            get { return currentDate; }
            set  { 
                this.SetField(ref currentDate, value, "CurrentDate"); 
            }
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
		
		public IList<DayInfo> Days { get; set; }
		
	}
}

