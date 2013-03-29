using iSeconds.Domain.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;

namespace iSeconds.Domain
{
	public class TimelineViewModel : ViewModel
	{
		private Timeline timeline = null;
		private IRepository repository = null;
        private IMediaService mediaService = null;
        private INavigator navigator = null;
		
		public TimelineViewModel(Timeline timeline, IRepository repository, IMediaService mediaService, INavigator navigator)
		{
			this.timeline = timeline;
			this.repository = repository;
            this.mediaService = mediaService;
            this.navigator = navigator;

            CalendarMode = VisualizationMode.MONTH;
			
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

                List<DayViewModel> viewModels = new List<DayViewModel>();

                List<Day> viewedDays = calendarMonth.GetViewedDays();
                foreach (Day date in viewedDays)
                {
                    DayViewModel viewModel = new DayViewModel(
                        this.timeline.GetDayAt(date.day), repository, this.mediaService, this.navigator
                    );

                    viewModel.PresentationInfo = date;
                    viewModels.Add(viewModel);
                }

                VisibleDays = viewModels;

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


        List<DayViewModel> visibleDays = new List<DayViewModel>();
        public List<DayViewModel> VisibleDays
        {
            get { return visibleDays; }
            set {
                this.SetField(ref visibleDays, value, "VisibleDays");
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

