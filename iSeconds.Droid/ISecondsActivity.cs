using Android.App;
using Android.OS;
using Android.Text;
using Android.Widget;
using LegacyBar.Library.Bar;
using iSeconds.Domain.Framework;
using LegacyBar.Library.BarActions;
using Android.Views;

namespace iSeconds.Droid
{
	public class ISecondsActivity : Activity
	{
		#if YESTER_LITE
		private YesterAdView adView = null;
		#endif

		protected INavigator navigator = null;
		protected ActivityTracker activityTracker = null;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			this.DisableBackButtonNavigation = false;
			ISecondsApplication application = this.Application as ISecondsApplication;
			activityTracker = application.GetActivityTracker();
			navigator = application.GetNavigator();
			activityTracker.SetCurrentActivity(this);
		}

		protected void setupAds()
		{
			#if YESTER_LITE
			LinearLayout layout= FindViewById<LinearLayout>(Resource.Id.yester_ads_layout);
			adView= new YesterAdView(this, layout);
			#endif
		}

		protected override void OnResume()
		{
			base.OnResume();

			#if YESTER_LITE
			if (adView != null)
				adView.OnResume();
			#endif

			activityTracker.SetCurrentActivity(this);
		}

		protected override void OnPause()
		{
			#if YESTER_LITE
			if (adView != null)
				adView.OnPause();
			#endif

			clearReferences();
			base.OnPause();
		}

		protected override void OnDestroy()
		{
			#if YESTER_LITE
			if (adView != null)
				adView.OnDestroy();
			#endif

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

			ScrollingTextView titleView = actionBar.FindViewById<ScrollingTextView>(Resource.Id.actionbar_title);
			titleView.TextSize = 26f;
			if (title.Length != 0)
				titleView.Text = title;

			titleView.Ellipsize = TextUtils.TruncateAt.Marquee;
			titleView.SetSingleLine();
			titleView.SetMarqueeRepeatLimit(0);

			if (addHomeAction)
			{
				var homeAction = new ActionLegacyBarAction(this, () => navigator.NavigateBack(), Resource.Drawable.ic_home);
				actionBar.SetHomeAction(homeAction);
			}
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