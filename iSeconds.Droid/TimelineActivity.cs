using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using LegacyBar.Library.Bar;
using iSeconds.Domain;
using System.ComponentModel;
using Android.Graphics.Drawables;

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

			configureActionBar(false);
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

		private void addActionBarItems()
		{
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);

			var takeVideoAction = new MenuItemLegacyBarAction(
				this, this, Resource.Id.actionbar_takeVideo, Resource.Drawable.ic_camera,
				Resource.String.takeVideo)
			{
				ActionType = ActionType.Always
			};			

			var moreAction = new MenuItemLegacyBarAction(
				this, this, Resource.Id.actionbar_more, Resource.Drawable.ic_action_overflow_dark,
				Resource.String.more)
			{
				ActionType = ActionType.Always
			};

			actionBar.AddAction(takeVideoAction);
			actionBar.AddAction(moreAction);

		}

		private void setupCalendar()
		{
			setActionBarTitle();

			TextView monthLabel = this.FindViewById<TextView> (Resource.Id.calendarMonthName);
			TextViewUtil.ChangeForDefaultFont(monthLabel, this, 18f);
			monthLabel.Text = this.viewModel.CurrentMonthTitle;

			CalendarMonthViewWeekNames monthWeekNames =
				FindViewById<CalendarMonthViewWeekNames> (Resource.Id.calendarWeekDays);
			List<DayViewModel> weekDays = new List<DayViewModel> (viewModel.VisibleDays.GetRange (0, 7));
			monthWeekNames.WeekDays = weekDays;

			monthView = FindViewById<CalendarMonthView> (Resource.Id.calendarView);
			monthView.ViewedDays = viewModel.VisibleDays;
			monthView.ViewModel = viewModel;

			this.viewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
				{
					if (e.PropertyName == "CurrentMonthTitle") {
						monthLabel.Text = this.viewModel.CurrentMonthTitle;
					}

					if (e.PropertyName == "VisibleDays") {
						monthView.ViewedDays = viewModel.VisibleDays;
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


		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{			
			case Resource.Id.actionbar_takeVideo:
				OnSearchRequested ();
				viewModel.TakeVideoCommand.Execute(null);				
				return true;
			case Resource.Id.actionbar_more:
				OnSearchRequested ();
				showPopup ();
				return true;
			}

			return base.OnOptionsItemSelected(item);
		}

		private void showPopup()
		{
			LinearLayout moreContentView = (LinearLayout) this.LayoutInflater.Inflate(Resource.Layout.OverflowMenu, null);

			moreContentView.Measure(Android.Views.View.MeasureSpec.MakeMeasureSpec (0, MeasureSpecMode.Unspecified)
			                        , Android.Views.View.MeasureSpec.MakeMeasureSpec (0, MeasureSpecMode.Unspecified));
			PopupWindow popupWindow = new PopupWindow(this);
			popupWindow.SetBackgroundDrawable(new ColorDrawable(this.Resources.GetColor(Resource.Color.white))); // acaba sendo a cor que da a impressao de bordas
			popupWindow.ContentView = moreContentView;
			// xunxo para pegar o ImageView do overflow adicionado como action pelo LegacyBar
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
			TextView titleView = actionBar.FindViewById<TextView>(Resource.Id.actionbar_title);
			var layout = actionBar.FindViewById<LinearLayout>(Resource.Id.actionbar_actions);
			View view = layout.GetChildAt (layout.ChildCount - 1); // pegamos o ultimo... nao consegui fazer de outro jeito..
			ImageView imageView = (ImageView)view;
			imageView.Selected = true;
			popupWindow.ShowAsDropDown (view);


			popupWindow.Touchable = true;
			popupWindow.Focusable = true;
			popupWindow.OutsideTouchable = true;
			popupWindow.DismissEvent += (object sender, EventArgs e) => {
				imageView.Selected = false;
			};

			Button settingsButton = moreContentView.FindViewById<Button>(Resource.Id.main_more_content_settings);
			settingsButton.Click += (object sender, EventArgs e) =>  {

				viewModel.SettingsCommand.Execute(null);
				popupWindow.Dismiss();

			};

			Button timelineOptionsButton = moreContentView.FindViewById<Button>(Resource.Id.main_more_content_timeline_options);
			timelineOptionsButton.Click += (object sender, EventArgs e) => {
				viewModel.OptionsCommand.Execute(null);
				popupWindow.Dismiss();
			};

			Button shareButton = moreContentView.FindViewById<Button>(Resource.Id.main_more_content_share);
			shareButton.Click += (object sender, EventArgs e) => {
				viewModel.ShareCommand.Execute(null);
				popupWindow.Dismiss();
			};

			Button aboutButton = moreContentView.FindViewById<Button>(Resource.Id.main_more_content_about);
			aboutButton.Click += (object sender, EventArgs e) => {
				// TODO:
				popupWindow.Dismiss();
			};

			popupWindow.Update(moreContentView.MeasuredWidth, moreContentView.MeasuredHeight);
		}

		public override void OnWindowFocusChanged (bool hasFocus)
		{
			base.OnWindowFocusChanged (hasFocus);
			// precisei fazer pois depois do menu de contexto aberto, se apertarmos o back button a seleção não é removida
			if (hasFocus)
				monthView.Invalidate();
		}


	}
}

