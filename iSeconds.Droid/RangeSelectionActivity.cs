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
using LegacyBar.Library.BarActions;
using iSeconds.Domain;
using Google.Analytics.Tracking;

namespace iSeconds.Droid
{
	[Activity (Label = "RangeSelectionActivity")]			
	public class RangeSelectionActivity : BaseTimelineActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			viewModel.OnRangeSelectionMode = true;
			configureSelectedDay ();

			View actionbarBottomLine = this.FindViewById<View>(Resource.Id.actionbar_bottom_line);
			actionbarBottomLine.SetBackgroundColor(Resources.GetColor(Resource.Color.actionbar_bottom_line_color_alt));
		}

      protected override void OnStart()
      {
         base.OnStart();
         EasyTracker.GetInstance(this).ActivityStart(this);
      }

      protected override void OnStop()
      {
         base.OnStop();
         EasyTracker.GetInstance(this).ActivityStop(this);
      }

		protected override bool showHomeButton()
		{
			return true;
		}

		protected override string getActivityTitle ()
		{
			return "";
		}

		protected override void addActionBarItems()
		{
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);

         var playAction = new ActionLegacyBarAction (this, () => {
            AnalyticsUtils.LogButtonEvent(this, "play_selection");
            viewModel.PlaySelectionCommand.Execute (null);
         }, Resource.Drawable.ic_play);
			playAction.ActionType = ActionType.Always;
			actionBar.AddAction (playAction);
			
         var compileAction = new ActionLegacyBarAction (this, () => {
            AnalyticsUtils.LogButtonEvent(this, "compile_selection");
            viewModel.CompileCommand.Execute (null);
         }, Resource.Drawable.ic_compile);
			compileAction.ActionType = ActionType.Always;
			actionBar.AddAction (compileAction);
			
         var rangeSelector = new ActionLegacyBarAction (this, () => { 
            AnalyticsUtils.LogButtonEvent(this, "open_range_selector");
				TimelineOptionsPopup.OpenRangeSelector (this, viewModel);
			}, Resource.Drawable.ic_calendar);
			rangeSelector.ActionType = ActionType.Always;
			actionBar.AddAction (rangeSelector);
		}

		void configureSelectedDay ()
		{
			if (this.Intent.HasExtra("SelectedDay")) {
				int day = Convert.ToInt32(this.Intent.Extras.GetString("SelectedDay"));
				int month = Convert.ToInt32(this.Intent.Extras.GetString("SelectedMonth"));
				int year = Convert.ToInt32(this.Intent.Extras.GetString("SelectedYear"));
				DateTime selectedDay = new DateTime(year, month, day);
				this.viewModel.RangeSelectionCommand.Execute(selectedDay);
			}

			if (this.Intent.HasExtra("CurrentMonth")) {
				int currentMonth = Convert.ToInt32(this.Intent.Extras.GetString ("CurrentMonth"));
				int currentYear = Convert.ToInt32(this.Intent.Extras.GetString ("CurrentYear"));
				this.viewModel.CurrentDate = new DateTime(currentYear, currentMonth, 1);
			}
		}
	}
}

