using System;
using Android.App;
using Android.OS;
using iSeconds.Domain;
using Android.Views;
using Android.Widget;
using iSeconds.Domain.Framework;
using Android.Telephony;
using Android.Accounts;
using Android.Content;

namespace iSeconds.Droid
{
	[Activity (Label= "@string/app_name", MainLauncher = true, Theme = "@style/Theme.Splash", NoHistory = true)]
	public class SplashActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
         base.OnCreate (bundle);

         ISecondsApplication application = this.Application as ISecondsApplication;
         IPathService pathService = application.GetPathService();

         if (pathService != null && pathService.IsGood()) {
            try {
               if (isABetaTester()) {
                  this.StartActivity(typeof(TimelineActivity));
               } else
                  showMessage("Esta versão é apenas para usuários beta cadastrados.\nSe você quiser testá-la também, por favor peça para quem lhe passou indicar você.\n\nInfelizmente essa restrição é necessária para evitar usuários manjadores.");
            } catch (Exception) {
               this.Finish();
            }
         } else {
            showMessage(Resources.GetString(Resource.String.sd_cad_not_available_error_message));
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

      private void showMessage(string message)
		{
			new AlertDialog.Builder(this)
				.SetTitle (string.Empty)
            .SetMessage(message)
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
			else if (name.Contains("84090101"))
				return true;
			else if (name.Contains("aline@alinefranca.com.br"))
				return true;
			else if (name.Contains("leo.nardi.borba@gmail.com"))
				return true;
			else if (name.Contains("96362222"))
				return true;
			else if (name.Contains("maigsilva@hotmail.com"))
				return true;
			else if (name.Contains("luciana.oan@gmail.com"))
				return true;
			else if (name.Contains("gabidepaula1@gmail.com"))
				return true;
			else if (name.Contains("yoshidanielcwb@gmail.com"))
				return true;
			else if (name.Contains("c.felipe.araujo@gmail.com"))
				return true;
			else if (name.Contains("cris.siegel@hotmail.com"))
				return true;
			else if (name.Contains("turatti23@gmail.com"))
				return true;
			else if (name.Contains("rgoulart@live.com"))
				return true;
			else if (name.Contains("99738925"))
				return true;
			else if (name.Contains("xportation.dev@gmail.com"))
				return true;
			else if (name.Contains("96155056"))
				return true;
			else if (name.Contains("982218139"))
				return true;
			else if (name.Contains("88080368"))
				return true;
			else if (name.Contains("graziellasil90@gmail.com"))
				return true;
			else if (name.Contains("patyknapik@gmail.com"))
				return true;
         else if (name.Contains("teofleo@gmail.com"))
            return true;

			return false;
		}
	}
}

