using Android.App;
using Android.OS;
using Android.Views;
using iSeconds.Domain;
using Android.Content;
using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using Android.Widget;
using System.Windows.Input;
using Android.Graphics;
using Google.Analytics.Tracking;

namespace iSeconds.Droid
{
	[Activity(Label = "CompilationActivity")]
	public class CompilationActivity : ISecondsActivity
	{
		private CompilationViewModel viewModel = null; 

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			this.RequestWindowFeature(WindowFeatures.NoTitle);
			this.SetContentView(Resource.Layout.CompilationView);
			configureActionBar(true, "");

			ISecondsApplication app = (ISecondsApplication)this.Application;
			viewModel = new CompilationViewModel (app.GetUserService ().CurrentUser, app.GetMediaService(),
				app.GetOptionsDialogService(), app.GetNavigator(), app.GetI18nService(), app.GetRepository());

			ListView list = this.FindViewById<ListView>(Resource.Id.compilationsList);
			ISecondsListViewAdapter adapter = new ISecondsListViewAdapter (this, viewModel.Compilations);
			list.Adapter = adapter;
			adapter.OnGetItem = delegate (int position, ListItemViewModel model, View view) {

				CompilationViewModel.CompilationItemViewModel compilationModel = (CompilationViewModel.CompilationItemViewModel)model;

				view = this.LayoutInflater.Inflate (Resource.Layout.CompilationViewItem, null);

				TextView name = view.FindViewById<TextView>(Resource.Id.textViewTimelineName);
				name.Text = compilationModel.Name;

				TextView begin = view.FindViewById<TextView>(Resource.Id.textViewDateBegin);
				begin.Text = GetString(Resource.String.compilation_item_date_begin) + " " + compilationModel.BeginDate;

				TextView end = view.FindViewById<TextView>(Resource.Id.textViewDateEnd);
				end.Text = GetString(Resource.String.compilation_item_date_end) + " " + compilationModel.EndDate;

				ImageView imageView = view.FindViewById<ImageView>(Resource.Id.compilationThumbnail);
				if (compilationModel.ThumbnailPath != null && compilationModel.ThumbnailPath.Length > 0) {
					Bitmap thumbnail = BitmapFactory.DecodeFile(compilationModel.ThumbnailPath);
					imageView.SetImageBitmap(thumbnail);
				}

				TextView fileSize = view.FindViewById<TextView>(Resource.Id.textViewCompilationSize);
				fileSize.Text= compilationModel.CompilationSize;

				ProgressBar progress= view.FindViewById<ProgressBar>(Resource.Id.compilationSpin);
				if (compilationModel.Done)
					progress.Visibility = ViewStates.Gone;
				else
					progress.Visibility = ViewStates.Visible;

				return view;
			};

			list.ItemClick += (sender, e) => viewModel.ShowCompilationOptionsCommand.Execute(e.Position);

			viewModel.OnCompilationViewModelChanged += (sender, args) => {
				RunOnUiThread(delegate { adapter.Invalidate(); });
			};

			setupAds();
		}

      protected override void OnStart()
      {
         base.OnStart();
         EasyTracker.GetInstance(this).ActivityStart(this);
      }

      protected override void OnStop()
      {
         base.OnStop();
         EasyTracker.GetInstance(this).ActivityStop(this);
      }

		protected override void OnDestroy()
		{
			base.OnDestroy();
			viewModel.Disconnect();
		}
	}
}

