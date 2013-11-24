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
			configureFonts();
			configureRadioButton();
		}
		
		void configureFonts()
		{
			TextView titleView= FindViewById<TextView>(Resource.Id.settingsTitle);
			TextView descriptionView= FindViewById<TextView>(Resource.Id.settingsDescription);
			RadioButton smallView= FindViewById<RadioButton>(Resource.Id.settingsVideoSizeSmall);
			RadioButton mediumView= FindViewById<RadioButton>(Resource.Id.settingsVideoSizeMedium);
			RadioButton largeView= FindViewById<RadioButton>(Resource.Id.settingsVideoSizeLarge);

			TextViewUtil.ChangeForDefaultFont(titleView,this,22f);
			TextViewUtil.ChangeForDefaultFont(descriptionView,this,18f);
			TextViewUtil.ChangeForDefaultFont(smallView,this,18f);
			TextViewUtil.ChangeForDefaultFont(mediumView,this,18f);
			TextViewUtil.ChangeForDefaultFont(largeView,this,18f);
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
	}
}

