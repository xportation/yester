using Android.App;
using Android.OS;
using Android.Widget;
using LegacyBar.Library.Bar;

namespace iSeconds.Droid
{
	public class ISecondsActivity : Activity
	{
		protected ActivityTracker activityTracker = null;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			activityTracker = ((ISecondsApplication) this.Application).GetActivityTracker();
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

		private void clearReferences()
		{
			Activity currActivity = activityTracker.GetCurrentActivity();
			if (currActivity != null && currActivity.Equals(this))
				activityTracker.SetCurrentActivity(null);
		}

		protected void configureActionBar(bool addHomeAction)
		{
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);

			actionBar.SeparatorColorRaw = Resource.Color.actionbar_background;

			TextView titleView = actionBar.FindViewById<TextView>(Resource.Id.actionbar_title);
			TextViewUtil.ChangeFontForActionBarTitle(titleView, this, 26f);

			ScrollingTextView sTextView = (ScrollingTextView)titleView;

			// removendo esse efeito horroroso...
			sTextView.SetHorizontallyScrolling (false);

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
	}
}