using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using iSeconds.Domain;
using LegacyBar.Library.Bar;
using System.Globalization;
using iSeconds.Domain.Framework;
using Android.Graphics;
using System;
using LegacyBar.Library.BarActions;

namespace iSeconds.Droid
{
   class VideoListAdapter : BaseAdapter
   {
		private Activity context = null;
      private DayOptionsViewModel viewModel = null;

      public VideoListAdapter (Activity context, DayOptionsViewModel viewModel)
      {
         this.viewModel = viewModel;
         this.context = context;

			foreach(DayOptionsViewModel.VideoItem model in viewModel.Videos) 
			{
				model.OnCheckedChanged += (object sender, GenericEventArgs<bool> args) =>
				{
					this.NotifyDataSetChanged();
				};
			}

			this.viewModel.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) => {
				if (e.PropertyName == "Videos")
					this.NotifyDataSetChanged();
			};
      }

      public override Java.Lang.Object GetItem (int position)
      {
         return null;
      }

      public override long GetItemId (int position)
      {
         return position;
      }

      public override View GetView (int position, View convertView, ViewGroup parent)
      {
         View view = convertView;
			TextView fileSize = null;
			CheckedTextView checkBox = null;
			DayOptionsViewModel.VideoItem model = viewModel.Videos[position];

         if (view == null) 
         {
            view = context.LayoutInflater.Inflate(Resource.Layout.DayOptionsItem, null);

				checkBox = view.FindViewById<CheckedTextView>(Resource.Id.videoItem);
            TextViewUtil.ChangeForDefaultFont(checkBox, context, 20f);

				fileSize = view.FindViewById<TextView>(Resource.Id.textViewDayOptionItemSize);
				TextViewUtil.ChangeForDefaultFont(fileSize, context, 14f);
         }

			checkBox = view.FindViewById<CheckedTextView>(Resource.Id.videoItem);

			checkBox.Text = model.Label;
			checkBox.Checked = model.Checked;
			
			ImageView imageView = view.FindViewById<ImageView> (Resource.Id.videoThumbnail);
			Bitmap thumbnail = BitmapFactory.DecodeFile(model.Model.GetThumbnailPath());
			imageView.SetImageBitmap(thumbnail);

			fileSize = view.FindViewById<TextView>(Resource.Id.textViewDayOptionItemSize);
			fileSize.Text = ISecondsUtils.FileSizeFormated(model.Model.Path);

         return view;
      }

      public override int Count {
         get { return viewModel.Videos.Count; }
      }
   }

   [Activity(Label = "Day options")]
   public class DayOptionsActivity : ISecondsActivity
   {
		private ListView listView = null;
		private VideoListAdapter adapter = null;
      private DayOptionsViewModel viewModel = null;

      protected override void OnCreate (Bundle bundle)
      {
         base.OnCreate (bundle);
			
			this.RequestWindowFeature(WindowFeatures.NoTitle);
			this.SetContentView (Resource.Layout.DayOptions);

         int day = Convert.ToInt32 (this.Intent.Extras.GetString ("Day"));
         int month = Convert.ToInt32 (this.Intent.Extras.GetString ("Month"));
         int year = Convert.ToInt32 (this.Intent.Extras.GetString ("Year"));
         int timelineId = Convert.ToInt32 (this.Intent.Extras.GetString ("TimelineId"));

			ISecondsApplication application = (ISecondsApplication)this.Application;
			User currentUser = application.GetUserService ().CurrentUser;
         Timeline timeline = currentUser.GetTimelineById (timelineId);
         DayInfo dayInfo = timeline.GetDayAt (new DateTime (year, month, day));

			INavigator navigator = application.GetNavigator();
			IMediaService mediaService = application.GetMediaService();
			IOptionsDialogService optionsDialogService = application.GetOptionsDialogService();
			I18nService i18n = application.GetI18nService();

			viewModel = new DayOptionsViewModel (timeline, dayInfo, navigator, mediaService, optionsDialogService, i18n);

			adapter = new VideoListAdapter (this, viewModel);
			listView = this.FindViewById<ListView> (Resource.Id.videosList);
         listView.Adapter = adapter;
			listView.ItemClick += (sender, e) => viewModel.PlayVideoCommand.Execute(e.Position);
			listView.ItemLongClick += (sender, e) => viewModel.ShowVideoOptionsCommand.Execute (e.Position);

			configureActionBar(true, getTitle());
			addActionBarItems ();

			Toast.MakeText(this, Resource.String.item_options_message, ToastLength.Long).Show();
      }

		private void addActionBarItems ()
		{
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);

			var takeVideoAction = new ActionLegacyBarAction(this, () => viewModel.TakeVideoCommand.Execute(null), Resource.Drawable.ic_camera);
			takeVideoAction.ActionType = ActionType.Always;
			actionBar.AddAction (takeVideoAction);
		}

		private string getTitle()
		{
			DateTimeFormatInfo format = CultureInfo.CurrentCulture.DateTimeFormat;
			return viewModel.Model.Date.ToString(format.ShortDatePattern);
		}

   }
}