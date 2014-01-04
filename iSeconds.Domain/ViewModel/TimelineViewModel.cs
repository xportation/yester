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
			// for�amos recalcular o viewmodels dos dias vis�veis
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
			get { return new Command((object arg) => { navigator.NavigateTo("play_selector", new Args()); }); }
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
						OnPropertyChanged("RangeSelection");
					}
				);
			}
		}

		private HashSet<DateTime> selectedDays = new HashSet<DateTime>();
		public HashSet<DateTime> SelectedDays 
		{
			get { return selectedDays; }
		}		

		public ICommand CompileCommand {
			get { return new Command((object arg) => { 
					Tuple<DateTime, DateTime> rangeDelimiters = getRangeDelimiters();

					string defaultName = timeline.Name + " (" + ISecondsUtils.DateToString(rangeDelimiters.Item1, false) + 
					                     " - " + ISecondsUtils.DateToString(rangeDelimiters.Item2, false) + ")";
					string defaultDescription = string.Format(i18n.Msg("A compilation for timeline {0} from {1} to {2}"), timelineName, 
						ISecondsUtils.DateToString(rangeDelimiters.Item1, false), ISecondsUtils.DateToString(rangeDelimiters.Item2, false));

					dialogService.AskForCompilationNameAndDescription(defaultName, defaultDescription, 
						(string name, string description) => {
							this.startCompilation(name, description, rangeDelimiters.Item1, rangeDelimiters.Item2);
						}, null);
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
			user.AddCompilation (compilation);

			dialogService.ShowMessage(
				i18n.Msg("Your compilation is now being processed..."), 
				() => mediaService.ConcatMovies(compilationPath, startDate, endDate, timeline.Id, user.UsesOnlyDefaultVideo)
			);
		}

		public ICommand CompilationsCommand {
			get { return new Command((object arg) => { navigator.NavigateTo("compilations_view", new Args()); }); }
		}

		public ICommand ShowTutorialCommand {
			get { return new Command((object arg) => { 
				if (!user.TutorialShown)
					dialogService.ShowTutorial( () => user.SetTutorialShown(true));
				}); 
			}
		}
	}
}
