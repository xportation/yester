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
using LegacyBar.Library.Bar;
using iSeconds.Domain;
using System.ComponentModel;

namespace iSeconds.Droid
{
	[Activity (Label = "TimelineActivity")]			
	public class TimelineActivity : ISecondsActivity
	{
		private CalendarMonthView monthView;
		private TimelineViewModel viewModel;

		private const string CurrentDateState= "currenteDateState";
		private const int ShowOptionsMenu= 101;


		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			this.RequestWindowFeature(WindowFeatures.NoTitle);
			this.SetContentView(Resource.Layout.TimelineView);

			ISecondsApplication application = (ISecondsApplication) this.Application;
			viewModel = new TimelineViewModel(application.GetUserService().CurrentUser, application.GetRepository(), 
			                                  application.GetMediaService(), application.GetNavigator());

			if (bundle != null) {
				viewModel.CurrentDate= DateTime.FromBinary(bundle.GetLong(CurrentDateState));
			}

			adjustActionBarTitle();
			addActionBarItems();
			setupCalendar();
		}

		protected override void OnResume()
		{
			base.OnResume();
			viewModel.Invalidate();
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);

			outState.PutLong(CurrentDateState, viewModel.CurrentDate.ToBinary());
		}

		void adjustActionBarTitle ()
		{
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);

			TextView titleView= actionBar.FindViewById<TextView>(Resource.Id.actionbar_title);
			TextViewUtil.ChangeFontForActionBarTitle(titleView,this,26f);
		}

		private void addActionBarItems()
		{
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
			//actionBar.SetHomeLogo(Resource.Drawable.ic_logo);

			var timelineOptionsMenuItemAction = new MenuItemLegacyBarAction(
				this, this, Resource.Id.actionbar_timeline_menu_options, Resource.Drawable.ic_menu,
				Resource.String.timeline_menu_options)
			{
				ActionType = ActionType.IfRoom
			};

			actionBar.AddAction(timelineOptionsMenuItemAction);
			
			var settingsItemAction = new MenuItemLegacyBarAction(
				this, this, Resource.Id.actionbar_settings, Resource.Drawable.ic_settings,
				Resource.String.settings)
			{
				ActionType = ActionType.IfRoom
			};

			actionBar.AddAction(settingsItemAction);
		}

		private void setupCalendar()
		{
			setActionBarTitle();

			TextView monthLabel = this.FindViewById<TextView> (Resource.Id.calendarMonthName);
			TextViewUtil.ChangeFontForMonthTitle(monthLabel, this, 18f);
			monthLabel.Text = this.viewModel.CurrentMonthTitle;

			CalendarMonthViewWeekNames monthWeekNames =
				FindViewById<CalendarMonthViewWeekNames> (Resource.Id.calendarWeekDays);
			List<DayViewModel> weekDays = new List<DayViewModel> (viewModel.VisibleDays.GetRange (0, 7));
			monthWeekNames.WeekDays = weekDays;

			monthView = FindViewById<CalendarMonthView> (Resource.Id.calendarView);
			monthView.ViewedDays = viewModel.VisibleDays;
			monthView.ViewModel = viewModel;
			configureVisibleDaysListener();

			this.viewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
				{
					if (e.PropertyName == "CurrentMonthTitle") {
						monthLabel.Text = this.viewModel.CurrentMonthTitle;
					}

					if (e.PropertyName == "VisibleDays") {
						monthView.ViewedDays = viewModel.VisibleDays;
						configureVisibleDaysListener();
					}

					if (e.PropertyName == "TimelineName") {
						setActionBarTitle();
					}
				};

		}

		private void setActionBarTitle()
		{
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
			actionBar.Title = this.viewModel.TimelineName;
		}

		private void configureVisibleDaysListener()
		{
			foreach (DayViewModel day in viewModel.VisibleDays) {
				day.DayOptionsRequest.Raised += (object s, GenericEventArgs<DayViewModel.DayOptionsList> args) =>  {
					optionsList = args.Value;
					ShowDialog (ShowOptionsMenu);
				};
			}
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
			case Resource.Id.actionbar_timeline_menu_options:
				OnSearchRequested();
				viewModel.OptionsCommand.Execute(null);
				return true;
			case Resource.Id.actionbar_settings:
				OnSearchRequested();
				viewModel.SettingsCommand.Execute(null);
				return true;
			}

			return base.OnOptionsItemSelected(item);
		}

		public override void OnWindowFocusChanged (bool hasFocus)
		{
			base.OnWindowFocusChanged (hasFocus);
			// precisei fazer pois depois do menu de contexto aberto, se apertarmos o back button a seleção não é removida
			if (hasFocus)
				monthView.Invalidate();
		}

		#region Dialog Modal

		private DayViewModel.DayOptionsList optionsList;

		protected override Dialog OnCreateDialog(int dialogType)
		{
			if (dialogType == ShowOptionsMenu) {
				if (optionsList == null)
					return null;

				var builder = new AlertDialog.Builder(this);
				builder.SetTitle(string.Empty);
				builder.SetItems(optionsList.ListNames(), (sender, eventArgs) => optionsList.DayEntryClicked.Execute(eventArgs.Which));

				return builder.Create();
			}

			return null;
		}

		#endregion
	}
}

