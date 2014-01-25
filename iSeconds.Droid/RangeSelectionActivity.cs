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

			var playAction = new ActionLegacyBarAction (this, () => viewModel.PlaySelectionCommand.Execute (null), Resource.Drawable.ic_play);
			playAction.ActionType = ActionType.Always;
			actionBar.AddAction (playAction);
			var compileAction = new ActionLegacyBarAction (this, () => viewModel.CompileCommand.Execute (null), Resource.Drawable.ic_compile);
			compileAction.ActionType = ActionType.Always;
			actionBar.AddAction (compileAction);
			var rangeSelector = new ActionLegacyBarAction (this, () => TimelineOptionsPopup.OpenRangeSelector (this, viewModel), Resource.Drawable.ic_compile);
			rangeSelector.ActionType = ActionType.Always;
			actionBar.AddAction (rangeSelector);
		}

		void configureSelectedDay ()
		{
			if (this.Intent.HasExtra ("SelectedDay")) {

				int day = Convert.ToInt32(this.Intent.Extras.GetString ("SelectedDay"));
				int month = Convert.ToInt32(this.Intent.Extras.GetString ("SelectedMonth"));
				int year = Convert.ToInt32(this.Intent.Extras.GetString ("SelectedYear"));
				DateTime selectedDay = new DateTime (year, month, day);
				this.viewModel.RangeSelectionCommand.Execute (selectedDay);
				this.viewModel.CurrentDate = selectedDay;
			}
		}
	}
}

