using iSeconds.Domain.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;
using System.Diagnostics;

namespace iSeconds.Domain
{
	public class TimelineViewModel : ViewModel
	{
		private User user = null;
		private Timeline timeline = null;
		private IRepository repository = null;
		private IMediaService mediaService = null;
		private INavigator navigator = null;

		public TimelineViewModel(User user, IRepository repository, IMediaService mediaService, INavigator navigator)
		{
			this.user = user;
			this.repository = repository;
			this.mediaService = mediaService;
			this.navigator = navigator;

			setTimeline(user.CurrentTimeline);
			CalendarMode = VisualizationMode.MONTH;

			this.CurrentDate = DateTime.Today;

			this.user.OnCurrentTimelineChanged += (sender, e) => 
				{
					setTimeline(e.Value);
					this.Invalidate();
				};

			this.user.OnTimelineUpdated += (sender, e) => this.TimelineName= e.Value.Name;
		}

		private void setTimeline(Timeline timeline)
		{
			this.timeline = timeline;
			this.TimelineName = this.timeline.Name;
		}

		private DayViewModel currentDateViewModel = null;

		private DateTime currentDate;

		public DateTime CurrentDate
		{
			get { return currentDate; }
			set
			{
				// se mudou o mes ou o ano devemos mudar o title do mes
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
					viewModel.PropertyChanged+= (sender, e) => {
						if (e.PropertyName == "VideoRecorded" || e.PropertyName == "VideoAdded") 
							this.Invalidate();
					};

					viewModels.Add(viewModel);

					if (currentDate == date.day)
						currentDateViewModel = viewModel;
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

		public enum VisualizationMode
		{
			MONTH,
			WEEK
		}

		private VisualizationMode calendarMode;

		public VisualizationMode CalendarMode
		{
			get { return calendarMode; }
			set { this.SetField(ref calendarMode, value, "CalendarMode"); }
		}

		public void Invalidate()
		{
			// forçamos recalcular o viewmodels dos dias visíveis
			this.CurrentDate = this.currentDate;
		}

		private List<DayViewModel> visibleDays = new List<DayViewModel>();

		public List<DayViewModel> VisibleDays
		{
			get { return visibleDays; }
			set { this.SetField(ref visibleDays, value, "VisibleDays"); }
		}

		public ICommand NextMonthCommand
		{
			get { return new Command(delegate { this.CurrentDate = this.CurrentDate.AddMonths(1); }); }
		}

		public ICommand PreviousMonthCommand
		{
			get { return new Command(delegate { this.CurrentDate = this.CurrentDate.AddMonths(-1); }); }
		}

		public ICommand GoToTodayCommand
		{
			get { return new Command(delegate { this.CurrentDate = DateTime.Today; }); }
		}

		public ICommand OptionsCommand
		{
			get { return new Command((object arg) => { navigator.NavigateTo("timeline_options", new Args()); }); }
		}

		public ICommand SettingsCommand 
		{
			get { return new Command((object arg) => { navigator.NavigateTo("settings_view", new Args()); }); }
		}

		public ICommand ShareCommand
		{
			get { return new Command((object arg) => {
					Args args = new Args();
					args.Put("TimelineId", this.timeline.Id.ToString());
					navigator.NavigateTo("share_view", args); 
				}); 
			}
		}

		public ICommand TakeVideoCommand
		{
			get { return new Command((object arg) => {
					currentDateViewModel.RecordVideoCommand.Execute(null);
				}); 
			}
		}

		public ICommand AboutCommand {
			get { return new Command((object arg) => { navigator.NavigateTo("about_view", new Args()); }); }
		}

		public ICommand PlayCommand {
			get { return new Command((object arg) => { navigator.NavigateTo("range_selector", new Args()); }); }
		}

		public ICommand PlaySelectionCommand {
			get {
				return new Command ((object arg) => { 

					Tuple<DateTime, DateTime> rangeDelimiters = getRangeDelimiters();

					Args args = new Args();
					args.Put("ShareDate_Start", rangeDelimiters.Item1.ToBinary().ToString());
					args.Put("ShareDate_End", rangeDelimiters.Item2.ToBinary().ToString());
					args.Put("ShareDate_TimelineId", this.timeline.Id.ToString());

					this.navigator.NavigateTo("video_player", args);
				
				});
			}
		}

		private string currentMonthTitle;

		public string CurrentMonthTitle
		{
			get { return currentMonthTitle; }
			set { this.SetField(ref currentMonthTitle, value, "CurrentMonthTitle"); }
		}

		private string timelineName;

		public string TimelineName
		{
			get { return timelineName; }
			set { this.SetField(ref timelineName, value, "TimelineName"); }
		}

		private void selectDay(DateTime day, bool select)
		{
			if (select)
				selectedDays.Add(day);
			else
				selectedDays.Remove(day);
		}

		private Tuple<DateTime, DateTime> getRangeDelimiters()
		{
			Debug.Assert (range.Count == 2);
			DateTime[] dates = new DateTime[2];
			range.CopyTo (dates);

			Tuple<DateTime, DateTime> result = new Tuple<DateTime, DateTime> (
				dates [0] < dates [1] ? dates [0] : dates [1],
				dates [0] > dates [1] ? dates [0] : dates [1]
				);

			return result;
		}

		private HashSet<DateTime> range = new HashSet<DateTime>();
		public HashSet<DateTime> Range 
		{
			get {
				return range;
			}
		}

		private void selectRangeBetween (DateTime day1, DateTime day2)
		{
			foreach (DateTime day in EachDay(day1, day2)) {
				selectedDays.Add (day);
			}
		}

		public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
		{
			for(var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
				yield return day;
		}

		public ICommand ToggleSelectionDayCommand
		{
			get {
				return new Command ((object arg) => { 

					DateTime day = (DateTime)arg;

					if (range.Count == 2) {
						range.Clear();
						range.Add(day);
						selectedDays.Clear();
						selectedDays.Add(day);
						return;
					}

					if (range.Count < 2) {
						range.Add(day);
						selectedDays.Add(day);
					}
					if (range.Count == 2) {

						DateTime[] days = new DateTime[2];
						selectedDays.CopyTo(days);
						DateTime first = days[0] < days[1] ? days[0] : days[1];
						DateTime second = days[0] > days[1] ? days[0] : days[1];
						selectRangeBetween(first, second);
					}

				});
			}
		}

		public ICommand SelectOnlyCommand
		{
			get {
				return new Command ((object arg) => { 

					DateTime day = (DateTime)arg;

					selectedDays.Clear();
					selectDay(day, true);

				});
			}
		}

		private HashSet<DateTime> selectedDays = new HashSet<DateTime>();
		public HashSet<DateTime> SelectedDays 
		{
			get { return selectedDays; }
		}		

	}
}