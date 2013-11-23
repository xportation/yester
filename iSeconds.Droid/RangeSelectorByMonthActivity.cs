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
using iSeconds.Domain;
using LegacyBar.Library.Bar;
using System.Globalization;

namespace iSeconds.Droid
{
	class RangeSelectorByMonthViewModel : ViewModel 
	{
		private Timeline timeline = null;

		public RangeSelectorByMonthViewModel(Timeline timeline) 
		{
			this.timeline = timeline;
			IList<int> yearsWithContent = timeline.GetYearsWithContent ();

			foreach (int year in yearsWithContent) {

				ListItemViewModel item = new ListItemViewModel (year.ToString(), null);
				years.Add (item);

				IList<int> months = timeline.GetMonthsWithContent (year);
				foreach (int month in months) {
					item.Children.Add(new ListItemViewModel(DateTimeFormatInfo.CurrentInfo.GetMonthName (month), null));		
				}
			}

		}

		private List<ListItemViewModel> years = new List<ListItemViewModel> ();
		public List<ListItemViewModel> Years 
		{
			get {
				return years;
			}
		}

		/*public ICommand PlayCommand 
		{
			get {
				return new Command((object arg) => {
					DateTime startDate = new DateTime (start.Year, start.Month + 1, start.DayOfMonth);
					DateTime endDate = new DateTime (end.Year, end.Month + 1, end.DayOfMonth);

					Intent intent= new Intent(this, typeof(VideoPlayerActivity));
					intent.PutExtra("ShareDate_Start", startDate.ToBinary());
					intent.PutExtra("ShareDate_End", endDate.ToBinary());
					intent.PutExtra("ShareDate_TimelineId", timelineId);
					this.StartActivity(intent);
				}); 

			}
		}*/
	}

	[Activity (Label = "RangeSelectorByMonthActivity")]			
	public class RangeSelectorByMonthActivity : ISecondsActivity
	{
		private RangeSelectorByMonthViewModel viewModel;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			this.RequestWindowFeature (WindowFeatures.NoTitle);
			this.SetContentView (Resource.Layout.ExpandableList);

			ISecondsApplication application = (ISecondsApplication) this.Application;

			viewModel = new RangeSelectorByMonthViewModel (application.GetUserService().CurrentUser.CurrentTimeline);

			ExpandableListView rangeOptionsList = this.FindViewById<ExpandableListView> (Resource.Id.expandableListView);
			List<ExpandableListAdapter.ExpandableGroup> contents = new List<ExpandableListAdapter.ExpandableGroup>();

			ExpandableListAdapter.ExpandableGroup group1 = new ExpandableListAdapter.ExpandableGroup("2012");
			group1.AddChild ("Janeiro");
			group1.AddChild ("Fevereiro");
			group1.AddChild ("Março");

			contents.Add (group1);

			ExpandableListAdapter.ExpandableGroup group2 = new ExpandableListAdapter.ExpandableGroup("2013");
			group2.AddChild ("Janeiro");
			group2.AddChild ("Fevereiro");
			group2.AddChild ("Março");

			contents.Add (group2);

			rangeOptionsList.SetAdapter(new ExpandableListAdapter(this, viewModel.Years));

			configureActionBar (true, "By month");
			addActionBarItems ();
		}

		private void addActionBarItems ()
		{
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar> (Resource.Id.actionbar);

			var playVideoAction = new MenuItemLegacyBarAction (
				this, this, Resource.Id.actionbar_playVideo, Resource.Drawable.ic_camera,
				Resource.String.play_video) {
				ActionType = ActionType.Always
			};

			actionBar.AddAction (playVideoAction);

		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{			
			case Resource.Id.actionbar_playVideo:
				OnSearchRequested ();
				//viewModel.PlaySelectionCommand.Execute(null);				
				//viewModel.PlayCommand.Execute (null);
				return true;
			}

			return base.OnOptionsItemSelected(item);
		}
	}
}

