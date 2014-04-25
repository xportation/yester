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
using Android.Gms.Ads;
using Android.Gms.Ads.Mediation.Admob;

namespace iSeconds.Droid
{
	public class YesterAdView
	{
		private AdView adView;
		private const string AD_UNIT_ID = "ca-app-pub-8299239170483597/9927382265";

		public YesterAdView(Context context, ViewGroup layout)
		{
			adView = new AdView(context);
			adView.AdUnitId = AD_UNIT_ID;
			adView.AdSize= AdSize.SmartBanner;

			layout.AddView(adView);

			Bundle mobExtras = new Bundle();
			mobExtras.PutString("color_bg", "FFFFFF");
			mobExtras.PutString("color_bg_top", "FFFFFF");
			mobExtras.PutString("color_border", "AAAAAA");
			mobExtras.PutString("color_link", "000080");
			mobExtras.PutString("color_text", "808080");
			mobExtras.PutString("color_url", "008000");

			AdMobExtras extras = new AdMobExtras(mobExtras);
			AdRequest adRequest = new AdRequest.Builder()
				.AddTestDevice(AdRequest.DeviceIdEmulator)
				.AddTestDevice("DDD499E69445741E89209945CC0B76AC")
				.AddTestDevice("4DB8EB0D373D32DE55001F588E1A6AD7")
				.AddNetworkExtras(extras)
				.Build();

			adView.LoadAd(adRequest);
		}

		public void OnResume()
		{
			adView.Resume();
		}

		public void OnPause()
		{
			adView.Pause();
		}

		public void OnDestroy()
		{
			adView.Destroy();
		}
	}
}

