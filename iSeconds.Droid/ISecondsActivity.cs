using Android.App;
using Android.OS;
using Android.Widget;
using LegacyBar.Library.Bar;
using iSeconds.Domain.Framework;
using Android.Text;

namespace iSeconds.Droid
{
	public class ISecondsActivity : Activity
	{
		protected INavigator navigator = null;
		protected ActivityTracker activityTracker = null;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			this.DisableBackButtonNavigation = false;
			ISecondsApplication application = (ISecondsApplication) this.Application;
			activityTracker = application.GetActivityTracker();
			navigator = application.GetNavigator();
			activityTracker.SetCurrentActivity(this);
		}

		protected override void OnResume()
		{
			base.OnResume();
			activityTracker.SetCurrentActivity(this);
		}

		protected override void OnPause()
		{
			clearReferences();
			base.OnPause();
		}

		protected override void OnDestroy()
		{
			clearReferences();
			base.OnDestroy();
		}

		public bool DisableBackButtonNavigation 
		{
			get;
			set;
		}

		private void clearReferences()
		{
			Activity currActivity = activityTracker.GetCurrentActivity();
			if (currActivity != null && currActivity.Equals(this))
				activityTracker.SetCurrentActivity(null);
		}

		protected void configureActionBar(bool addHomeAction, string title)
		{
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);

			actionBar.SeparatorColorRaw = Resource.Color.actionbar_background;

			TextView titleView = actionBar.FindViewById<TextView>(Resource.Id.actionbar_title);
			TextViewUtil.ChangeFontForActionBarTitle(titleView, this, 26f);
			if (title.Length != 0)
				titleView.Text = title;

			titleView.Ellipsize = TextUtils.TruncateAt.Marquee;
			titleView.SetSingleLine();
			titleView.SetMarqueeRepeatLimit(0);

			if (addHomeAction)
			{
				var itemActionBarAction = new MenuItemLegacyBarAction(this, this, Resource.Id.actionbar_back_to_home,
				                                                      Resource.Drawable.ic_home, Resource.String.settings)
					{
						ActionType = ActionType.Always
					};

				actionBar.SetHomeAction(itemActionBarAction);
			}
		}

		public override bool OnOptionsItemSelected(Android.Views.IMenuItem item)
		{
			switch (item.ItemId)
			{
			case Resource.Id.actionbar_back_to_home:
				OnSearchRequested();
				navigator.NavigateBack();
				return true;         
			}

			return base.OnOptionsItemSelected(item);
		}

		public override void OnBackPressed()
		{
			if (!this.DisableBackButtonNavigation)
				navigator.NavigateBack();
			else
				base.OnBackPressed();
		}
	}
}