using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using iSeconds.Domain;

namespace iSeconds.Droid
{
	[Activity (Label = "SettingsActivity")]			
	public class SettingsActivity : ISecondsActivity
	{
		private const int SMALL_SIZE = 1;
		private const int MEDIUM_SIZE = 3;
		private const int LARGE_SIZE = 5;
		private SettingsViewModel viewModel;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			this.RequestWindowFeature(WindowFeatures.NoTitle);
			this.SetContentView(Resource.Layout.SettingsView);

			ISecondsApplication application = (ISecondsApplication) this.Application;
			viewModel = new SettingsViewModel(application.GetUserService().CurrentUser);

			configureActionBar(true, "");
			configureRadioButton();
			configureCheckBox();
			setupAds();
		}

		void configureRadioButton()
		{
			RadioButton smallView= FindViewById<RadioButton>(Resource.Id.settingsVideoSizeSmall);
			RadioButton mediumView= FindViewById<RadioButton>(Resource.Id.settingsVideoSizeMedium);
			RadioButton largeView= FindViewById<RadioButton>(Resource.Id.settingsVideoSizeLarge);

			RadioButton radioButtonSelected= null;
			switch (viewModel.GetRecordDuration()) {
			case SMALL_SIZE: radioButtonSelected = smallView; break;
			case LARGE_SIZE: radioButtonSelected = largeView; break;
			default: radioButtonSelected = mediumView; break;
			}

			radioButtonSelected.Checked = true;
						
			smallView.Click+= (sender, e) => viewModel.ChangeTimeCommand.Execute(SMALL_SIZE);
			mediumView.Click+= (sender, e) => viewModel.ChangeTimeCommand.Execute(MEDIUM_SIZE);
			largeView.Click+= (sender, e) => viewModel.ChangeTimeCommand.Execute(LARGE_SIZE);
		}

		void configureCheckBox()
		{
			CheckBox checkOnlyDefaultVideo = FindViewById<CheckBox>(Resource.Id.settingsOnlyDefaultVideos);
			TextView liteMessage = FindViewById<TextView>(Resource.Id.settingsLiteWarning);
			#if YESTER_LITE
			checkOnlyDefaultVideo.Checked = true;
			checkOnlyDefaultVideo.Enabled = false;
			checkOnlyDefaultVideo.SetTextColor(Android.Graphics.Color.Gray);
			liteMessage.Visibility = ViewStates.Visible;
			#else
			checkOnlyDefaultVideo.Checked = viewModel.UsesOnlyDefaultVideo();
			checkOnlyDefaultVideo.CheckedChange+= (sender, e) => viewModel.ChangeUsesOnlyDefaultVideoCommand.Execute(e.IsChecked);
			liteMessage.Visibility = ViewStates.Gone;
			#endif
		}
	}
}

