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
using iSeconds.Domain.Framework;

namespace iSeconds.Droid
{
	class RangeSelectorViewModel : ViewModel
	{
		private List<ListItemViewModel> options = new List<ListItemViewModel> ();
		public List<ListItemViewModel> Options { get { return options; } }

		public RangeSelectorViewModel(INavigator navigator)
		{
			Options.Add (new ListItemViewModel("By Month", () => {
				navigator.NavigateTo("range_selector_by_month", new Args());
			}));
			Options.Add (new ListItemViewModel("By Range", () => {
				navigator.NavigateTo("range_selector_by_day", new Args());
			}));
		}
	}

	[Activity (Label = "RangeSelectorActivity")]			
	public class RangeSelectorActivity : ISecondsActivity
	{
		private RangeSelectorViewModel viewModel = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			this.RequestWindowFeature (WindowFeatures.NoTitle);
			this.SetContentView (Resource.Layout.RangeSelector);

			ISecondsApplication application = (ISecondsApplication) this.Application;

			viewModel = new RangeSelectorViewModel (application.GetNavigator());

			ListView rangeOptionsList = this.FindViewById<ListView> (Resource.Id.rangeOptionsList);

			rangeOptionsList.Adapter = new ISecondsListViewAdapter (this, viewModel.Options);	

			configureActionBar (true, "");
		}

	}
}

