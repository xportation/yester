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

			configureActionBar();
			configureFonts();
			configureRadioButton();
		}

		private void configureActionBar()
		{
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
			var itemActionBarAction = new MenuItemLegacyBarAction(this, this, Resource.Id.actionbar_back_to_home, 
			                                                      Resource.Drawable.ic_home, Resource.String.settings)
			{
				ActionType = ActionType.Always
			};

			actionBar.SetHomeAction(itemActionBarAction);
			actionBar.SeparatorColorRaw= Resource.Color.actionbar_background;

			TextView titleView= actionBar.FindViewById<TextView>(Resource.Id.actionbar_title);
			TextViewUtil.ChangeFontForActionBarTitle(titleView,this,26f);
		}

		void configureFonts()
		{
			TextView titleView= FindViewById<TextView>(Resource.Id.settingsTitle);
			TextView descriptionView= FindViewById<TextView>(Resource.Id.settingsDescription);
			RadioButton smallView= FindViewById<RadioButton>(Resource.Id.settingsVideoSizeSmall);
			RadioButton mediumView= FindViewById<RadioButton>(Resource.Id.settingsVideoSizeMedium);
			RadioButton largeView= FindViewById<RadioButton>(Resource.Id.settingsVideoSizeLarge);

			TextViewUtil.ChangeFontForSettingView(titleView,this,22f);
			TextViewUtil.ChangeFontForSettingView(descriptionView,this,18f);
			TextViewUtil.ChangeFontForSettingView(smallView,this,18f);
			TextViewUtil.ChangeFontForSettingView(mediumView,this,18f);
			TextViewUtil.ChangeFontForSettingView(largeView,this,18f);
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

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
			case Resource.Id.actionbar_back_to_home:
				OnSearchRequested();
				this.Finish();
				return true;         
			}

			return base.OnOptionsItemSelected(item);
		}
	}
}

