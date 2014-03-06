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
   public class SplashActivity : ISecondsActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
         base.OnCreate (bundle);

         ISecondsApplication application = this.Application as ISecondsApplication;
         IPathService pathService = application.GetPathService();

			if (isARTDevice()) {
				showMessage(Resources.GetString(Resource.String.not_compatible_with_art_device));
				return;
			}

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
			string[] namesSuported = { "xportation@gmail.com", "martim00@gmail.com", "biaosiegel@gmail.com", "ronald.paloschi@gmail.com", 
				"84090101", "aline@alinefranca.com.br", "leo.nardi.borba@gmail.com", "96362222", "maigsilva@hotmail.com", "luciana.oan@gmail.com", 
				"gabidepaula1@gmail.com", "yoshidanielcwb@gmail.com", "c.felipe.araujo@gmail.com", "cris.siegel@hotmail.com", "turatti23@gmail.com", 
				"rgoulart@live.com", "99738925", "xportation.dev@gmail.com", "96155056", "982218139", "88080368", "graziellasil90@gmail.com", 
				"patyknapik@gmail.com", "teofleo@gmail.com", "edjeanmsampaio@gmail.com" };

			foreach (string nameSuported in namesSuported) {
				if (name.Contains(nameSuported))
					return true;
			}

			return false;
		}

		private bool isARTDevice()
		{
			try {
				var systemProperties = Java.Lang.Class.ForName("android.os.SystemProperties");

				try {
					var str = new Java.Lang.String();
					var getMethod = systemProperties.GetMethod("get", str.Class, str.Class);
					if (getMethod == null) 
						return false;

					try {
						const string SELECT_RUNTIME_PROPERTY = "persist.sys.dalvik.vm.lib";
						var value = getMethod.Invoke(systemProperties, SELECT_RUNTIME_PROPERTY,
							/* Assuming default is */"Dalvik").ToString();
						if (value.Contains("ART")) 
							return true;

						return false;
					} 
					catch (Exception) {
						return false;
					}
				}
				catch (Exception) {
					return false;
				}
			} 
			catch (Exception) {
				return false;
			}
		}
	}
}

