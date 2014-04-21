using iSeconds.Domain.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;
using System.Diagnostics;
using System.Collections;
using System.IO;

namespace iSeconds.Domain
{
	public class TimelineViewModel : ViewModel
	{
		private User user = null;
		private Timeline timeline = null;
		private IRepository repository = null;
		private IMediaService mediaService = null;
		private INavigator navigator = null;
		private IOptionsDialogService dialogService = null;
		private I18nService i18n = null;
		private IPathService pathService = null;

		private EventHandler<GenericEventArgs<Timeline>> timelineUpdatedHandler;
		private EventHandler<GenericEventArgs<Timeline>> currentTimelineChangedHandler;

		public TimelineViewModel(User user, IRepository repository, IMediaService mediaService, 
			INavigator navigator, IOptionsDialogService dialogService, I18nService i18n, IPathService pathService)
		{
			this.user = user;
			this.repository = repository;
			this.mediaService = mediaService;
			this.navigator = navigator;
			this.dialogService = dialogService;
			this.i18n = i18n;
			this.pathService = pathService;

			setTimeline(user.CurrentTimeline);
			CalendarMode = VisualizationMode.MONTH;

			this.CurrentDate = DateTime.Today;

			timelineUpdatedHandler= new EventHandler<GenericEventArgs<Timeline>>((sender, e) => {
				if (e.Value.Id == this.timeline.Id)
					setTimeline(e.Value);
			});

			currentTimelineChangedHandler = new EventHandler<GenericEventArgs<Timeline>>((sender, e) => {
				setTimeline(e.Value);
				this.Invalidate();
			});

			this.user.OnTimelineUpdated += timelineUpdatedHandler;
			this.user.OnCurrentTimelineChanged += currentTimelineChangedHandler;
			this.onRangeSelectionMode = false;
		}

		public void Disconnect()
		{
			this.user.OnTimelineUpdated -= timelineUpdatedHandler;
			this.user.OnCurrentTimelineChanged -= currentTimelineChangedHandler;
		}

		private void setTimeline(Timeline timeline)
		{
			this.timeline = timeline;
			this.TimelineName = this.timeline.Name;
		}

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

				CalendarMonth calendarMonth = new CalendarMonth(true, value, i18n);

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
			string strMonthName = string.Empty;
			if (month == 1)
				strMonthName = i18n.Msg("January");
			else if (month == 2)
				strMonthName = i18n.Msg("February");
			else if (month == 3)
				strMonthName = i18n.Msg("March");
			else if (month == 4)
				strMonthName = i18n.Msg("April");
			else if (month == 5)
				strMonthName = i18n.Msg("May");
			else if (month == 6)
				strMonthName = i18n.Msg("June");
			else if (month == 7)
				strMonthName = i18n.Msg("July");
			else if (month == 8)
				strMonthName = i18n.Msg("August");
			else if (month == 9)
				strMonthName = i18n.Msg("September");
			else if (month == 10)
				strMonthName = i18n.Msg("October");
			else if (month == 11)
				strMonthName = i18n.Msg("November");
			else if (month == 12)
				strMonthName = i18n.Msg("December");

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
				DayViewModel currentDateViewModel = null;
				foreach (DayViewModel dayViewModel in this.VisibleDays) {
					if (dayViewModel.Model.Date == DateTime.Today) {
						currentDateViewModel= dayViewModel;
						break;
					}
				}

				if (currentDateViewModel == null)
					currentDateViewModel = new DayViewModel(this.timeline.GetDayAt(DateTime.Today), repository, this.mediaService, this.navigator);

				currentDateViewModel.RecordVideoCommand.Execute(null);
				}); 
			}
		}

		public ICommand AboutCommand {
			get { return new Command((object arg) => { navigator.NavigateTo("about_view", new Args()); }); }
		}

		public ICommand PlayCommand {
			get { return new Command((object arg) => { navigator.NavigateTo("play_selector", new Args()); }); }
		}

		public ICommand PlaySelectionCommand {
			get {
				return new Command ((object arg) => { 

					Tuple<DateTime, DateTime> rangeDelimiters = getRangeDelimiters();
					if (rangeDelimiters != null) {
						Args args = new Args();
						args.Put("ShareDate_Start", rangeDelimiters.Item1.ToBinary().ToString());
						args.Put("ShareDate_End", rangeDelimiters.Item2.ToBinary().ToString());
						args.Put("ShareDate_TimelineId", this.timeline.Id.ToString());

						this.navigator.NavigateTo("video_player", args);
					}
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

		private Tuple<DateTime, DateTime> getRangeDelimiters()
		{
			if (range.Count == 0)
				return null;

			DateTime[] dates = new DateTime[range.Count];
			range.CopyTo (dates);

			if (dates.Length == 1)
				return new Tuple<DateTime, DateTime>(dates[0], dates[0]);

			return new Tuple<DateTime, DateTime> (
					dates [0] < dates [1] ? dates [0] : dates [1],
					dates [0] > dates [1] ? dates [0] : dates [1]);
		}

		private SortedSet<DateTime> range = new SortedSet<DateTime>();
		public SortedSet<DateTime> Range 
		{
			get {
				return range;
			}
		}

		public void ClearSelection()
		{
			range.Clear();
			selectedDays.Clear();
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

		public ICommand RangeSelectionCommand
		{
			get {
				return new Command ((object arg) => 
					{ 
						DateTime day = (DateTime)arg;
						if (range.Count == 2) {
							this.ClearSelection();
							range.Add(day);
							selectedDays.Add(day);
						} else {
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
					}
				);
			}
		}

		private HashSet<DateTime> selectedDays = new HashSet<DateTime>();
		public HashSet<DateTime> SelectedDays 
		{
			get { return selectedDays; }
		}



		private bool onRangeSelectionMode;
		public bool OnRangeSelectionMode {
			get { return onRangeSelectionMode; }
			set {
				onRangeSelectionMode = value;
				OnPropertyChanged("RangeSelection");
			}
		}
		
		public ICommand LongPressCommand {
			get { 
				return new Command((object arg) => { 

					// se já estiver no modo de range selection apenas volta para a activity anterior
					if (OnRangeSelectionMode) {
						navigator.NavigateBack();
					}
					else { // senão vai iniciar a activity de range selection
						Args args = new Args();
						if (arg != null) {
							// se a data for passada já iniciamos a seleção de range com 1 dia
							DateTime selectedDate = (DateTime)arg;
							args.Put("SelectedDay", selectedDate.Day.ToString());
							args.Put("SelectedMonth", selectedDate.Month.ToString());
							args.Put("SelectedYear", selectedDate.Year.ToString());
						}

						// isso eh para conseguirmos lançar o range selector mode no mes certo
						args.Put("CurrentMonth", this.currentDate.Month.ToString());
						args.Put("CurrentYear", this.currentDate.Year.ToString());

						navigator.NavigateTo("range_selection", args);
					}
				});
			}
		}

		int getRangeCount(Tuple<DateTime, DateTime> rangeDelimiters)
		{
			var medias= repository.GetMediaInfoByPeriod(rangeDelimiters.Item1, rangeDelimiters.Item2, this.timeline.Id, true);
			return medias.Count;
		}

		public ICommand CompileCommand {
			get { 
				return new Command((object arg) => {
					Tuple<DateTime, DateTime> rangeDelimiters = getRangeDelimiters();
					if (rangeDelimiters != null) {
						#if YESTER_LITE
						int daysCount= getRangeCount(rangeDelimiters);
						if (daysCount > 7) {
							dialogService.ShowMessageLite(i18n.Msg("The lite version is limited to only seven days by compilation"), null);
							return;
						}
						#endif

						string defaultName = timeline.Name;
						string defaultDescription = string.Format(i18n.Msg("A compilation for timeline {0} from {1} to {2}"), timelineName, 
							ISecondsUtils.DateToString(rangeDelimiters.Item1, false), ISecondsUtils.DateToString(rangeDelimiters.Item2, false));

						dialogService.AskForCompilationNameAndDescription(defaultName, defaultDescription, 
							(string name, string description) => {
								this.startCompilation(name, description, rangeDelimiters.Item1, rangeDelimiters.Item2);
							}, null);
					}
				});
			}
		}

		private void startCompilation(string name, string description, DateTime startDate, DateTime endDate)
		{
			string basePath = Path.Combine(pathService.GetCompilationPath (), ISecondsUtils.StringifyDate("compilation", DateTime.Now));
			string compilationPath = basePath + ".mp4";
			string thumbnailPath = basePath + ".png";

			Compilation compilation = new Compilation ();
			compilation.Name = name;
			compilation.Description = description;
			compilation.Begin = startDate;
			compilation.End = endDate;
			compilation.TimelineName = timeline.Name;
			compilation.Path = compilationPath;
			compilation.ThumbnailPath = thumbnailPath;
			compilation.Done = false;
			user.AddCompilation (compilation);

			dialogService.ShowMessage(
				i18n.Msg("Your compilation is now being processed..."), 
				() => mediaService.ConcatMovies(compilationPath, startDate, endDate, timeline.Id, user.UsesOnlyDefaultVideo)
			);
		}

		public ICommand CompilationsCommand {
			get { return new Command((object arg) => { navigator.NavigateTo("compilations_view", new Args()); }); }
		}
	}
}
