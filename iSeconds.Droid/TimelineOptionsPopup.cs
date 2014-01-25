using System;
using Android.App;
using iSeconds.Domain;
using Android.Widget;
using Android.Views;
using Android.Graphics.Drawables;

namespace iSeconds.Droid
{
	public static class TimelineOptionsPopup
	{
		public static void Show(Activity activity, TimelineViewModel viewModel)
		{
			LinearLayout moreContentView = (LinearLayout) activity.LayoutInflater.Inflate(Resource.Layout.OverflowMenu, null);

			moreContentView.Measure(Android.Views.View.MeasureSpec.MakeMeasureSpec (0, MeasureSpecMode.Unspecified), 
				Android.Views.View.MeasureSpec.MakeMeasureSpec (0, MeasureSpecMode.Unspecified));
			PopupWindow popupWindow = new PopupWindow(activity);
			popupWindow.SetBackgroundDrawable(new ColorDrawable(activity.Resources.GetColor(Resource.Color.activity_background)));
			popupWindow.ContentView = moreContentView;
			// xunxo para pegar o ImageView do overflow adicionado como action pelo LegacyBar
			var actionBar = activity.FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
			var layout = actionBar.FindViewById<LinearLayout>(Resource.Id.actionbar_actions);
			View view = layout.GetChildAt (layout.ChildCount - 1); // pegamos o ultimo... nao consegui fazer de outro jeito..
			ImageView imageView = (ImageView)view;
			imageView.Selected = true;
			popupWindow.ShowAsDropDown (view);

			popupWindow.Touchable = true;
			popupWindow.Focusable = true;
			popupWindow.OutsideTouchable = true;
			popupWindow.DismissEvent += (object sender, EventArgs e) => {
				imageView.Selected = false;
			};

			Button settingsButton = moreContentView.FindViewById<Button>(Resource.Id.main_more_content_settings);
			settingsButton.Click += (object sender, EventArgs e) =>  {
				popupWindow.Dismiss();
				viewModel.SettingsCommand.Execute(null);
			};

			Button timelineOptionsButton = moreContentView.FindViewById<Button>(Resource.Id.main_more_content_timeline_options);
			timelineOptionsButton.Click += (object sender, EventArgs e) => {
				popupWindow.Dismiss();
				viewModel.OptionsCommand.Execute(null);
			};

			Button compileButton = moreContentView.FindViewById<Button>(Resource.Id.main_more_content_range_selector);
			compileButton.Click += (object sender, EventArgs e) => {
				popupWindow.Dismiss();
				viewModel.LongPressCommand.Execute(null);
			};

			Button aboutButton = moreContentView.FindViewById<Button>(Resource.Id.main_more_content_about);
			aboutButton.Click += (object sender, EventArgs e) => {
				popupWindow.Dismiss();
				viewModel.AboutCommand.Execute(null);
			};

			Button compilationsButton = moreContentView.FindViewById<Button>(Resource.Id.main_more_content_compilations);
			compilationsButton.Click += (object sender, EventArgs e) => {
				popupWindow.Dismiss();
				viewModel.CompilationsCommand.Execute(null);
			};

			popupWindow.Update(moreContentView.MeasuredWidth, moreContentView.MeasuredHeight);
		}

		public static void OpenRangeSelector(Activity activity, TimelineViewModel viewModel)
		{
			Dialog dialog = new Dialog(activity);
			dialog.SetContentView (Resource.Layout.DatePickerView);
			DatePicker start = dialog.FindViewById<DatePicker>(Resource.Id.dateStartPeriod);
			DatePicker end = dialog.FindViewById<DatePicker>(Resource.Id.dateEndPeriod);

			if (viewModel.Range.Count >= 1) {
				DateTime[] dateTime = new DateTime[viewModel.Range.Count];
				viewModel.Range.CopyTo(dateTime);
				start.SetDateTime(dateTime[0]);
				if (viewModel.Range.Count == 2)
					end.SetDateTime(dateTime[1]);
			}

			Button selectButton = dialog.FindViewById<Button> (Resource.Id.selectButton);
			selectButton.Click += (object sender, EventArgs e) => {
				DateTime startDate = new DateTime (start.Year, start.Month + 1, start.DayOfMonth);
				DateTime endDate = new DateTime (end.Year, end.Month + 1, end.DayOfMonth);

				viewModel.ClearSelection();
				viewModel.RangeSelectionCommand.Execute(startDate);
				viewModel.RangeSelectionCommand.Execute(endDate);

				dialog.Dismiss();
			};

			Button cancelButton = dialog.FindViewById<Button> (Resource.Id.cancelButton);
			cancelButton.Click += (object sender, EventArgs e) =>  {
				dialog.Dismiss();
			};

			dialog.Show();
		}
	}
}

