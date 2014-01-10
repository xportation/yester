using System;
using Android.App;
using Android.OS;
using iSeconds.Domain;
using Android.Views;
using Android.Widget;
using iSeconds.Domain.Framework;
using Android.Telephony;
using Android.Accounts;

namespace iSeconds.Droid
{
	[Activity (Label= "@string/app_name", MainLauncher = true, Theme = "@style/Theme.Splash", NoHistory = true)]
	public class SplashActivity : ISecondsActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
         try {
				if (isABetaTester())
					navigator.NavigateTo("timeline_view", new Args());
				else
					betaTesterRestricionMessage();
         } catch (Exception) {
            this.Finish();
         }
		}

		private bool isABetaTester()
		{
			AccountManager am = AccountManager.Get(this);
			Account[] accounts = am.GetAccounts();

			foreach (Account ac in accounts) {
				if (isAccountAllowed(ac.Name)) {
					return true;
				}
			}
			return false;
		}

		private void betaTesterRestricionMessage()
		{
			new AlertDialog.Builder(this)
				.SetTitle (string.Empty)
				.SetMessage ("Esta versão é apenas para usuários beta cadastrados.\nSe você quiser testá-la também, por favor peça para quem lhe passou indicar você.\n\nInfelizmente essa restrição é necessária para evitar usuários manjadores.")
				.SetPositiveButton(Resource.String.ok, delegate { this.Finish(); })
				.Show ();
		}

		private bool isAccountAllowed(string name)
		{
			if (name.Contains("xportation@gmail.com"))
				return true;
			else if (name.Contains("martim00@gmail.com"))
				return true;
			else if (name.Contains("biaosiegel@gmail.com"))
				return true;
			else if (name.Contains("ronald.paloschi@gmail.com"))
				return true;
			else if (name.Contains("84040101"))
				return true;
			else if (name.Contains("99232872"))
				return true;
			else if (name.Contains("99117797"))
				return true;
			else if (name.Contains("96362222"))
				return true;
			else if (name.Contains("88421366"))
				return true;

			return false;
		}
	}
}

