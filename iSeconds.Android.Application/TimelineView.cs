
using Android.Widget;
using CalendarControl;
using iSeconds.Domain;
using Android.Content;
using Android.Views;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Graphics;
using Android.App;
using Android.Support.V4.App;
using Android.Net;
using Android.Provider;
using Android.Media;
using Xamarin.Media;

namespace iSeconds
{
	public class TimelineFragment : Fragment
	{
		private UserService userService = null;
		private User actualUser = null;

		private void actualUserChanged (User newUser)
		{
			actualUser = newUser;
			actualUser.OnActualTimelineChanged+= (object sender, GenericEventArgs<Timeline> args) => {
				invalidateTimeline();
			};
		}
		public override void OnCreate (Bundle bundle)
		{
			base.OnCreate(bundle);

			this.userService = ((ISecondsApplication)this.Activity.Application).GetUserService();
			this.userService.OnActualUserChanged+= (object sender, GenericEventArgs<User> args) => {
				actualUserChanged(args.Value);
			};
			
			if (userService.ActualUser != null)
				actualUserChanged(userService.ActualUser);


		}

		private LinearLayout layout = null;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container,
		                                  Bundle savedInstanceState) {

			this.layout = new LinearLayout(this.Activity);
			this.layout.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, 
			                                                             LinearLayout.LayoutParams.FillParent);

			if (actualUser != null && actualUser.TimelineCount > 0)
				invalidateTimeline();

			return this.layout;
		}

		TimelineView actualView = null;
		void invalidateTimeline ()
		{
			Timeline timeline = actualUser.ActualTimeline;
			if (actualView == null || actualView.Timeline != timeline) {	
				
//				this.SupportFragmentManager.BeginTransaction()
//					.Add(Resource.Id.mainLayout, new TimelineView(timeline))
//						.Commit();
//				
				layout.RemoveView (actualView);
				actualView = new TimelineView (timeline, this.Activity, this);

				layout.AddView (actualView);
			}
		}
			

		
		public override void OnActivityResult (int requestCode, int resultCode, Intent data)
		{
			if (requestCode == ISecondsConstants.SELECT_PHOTO_RESULT && data != null && data.Data != null) {
				Uri _uri = data.Data;
				
				if (_uri != null) {
					
					//User had pick an image.
					var cursor = this.Activity.ContentResolver.Query (
						_uri, new string[] { MediaStore.Images.ImageColumns.Data }, null, null, null
					);
					cursor.MoveToFirst ();
					
					//Link to the image
					string imageFilePath = cursor.GetString (0);
					cursor.Close ();

					if (actualView != null)
						actualView.HandleImageQueryResult (imageFilePath);
				}
			} 

			base.OnActivityResult(requestCode, resultCode, data);
		}

		public void OpenGallery()
		{
			Intent photoPickerIntent = new Intent(Intent.ActionPick);
			photoPickerIntent.SetType("image/*");
			photoPickerIntent.PutExtra("return-data", true);
			
			this.StartActivityForResult(photoPickerIntent, ISecondsConstants.SELECT_PHOTO_RESULT);
		}

		private string generateName(string prefix, System.DateTime dateTime)
		{
			string movieName = prefix + "_" + dateTime.ToString();
			movieName = movieName.Replace("/", "_");
			movieName = movieName.Replace(" ", "_");
			movieName = movieName.Replace(":", "_");
			return movieName;
		}

		private static readonly System.Object obj = new System.Object();

		public void TakeVideo (System.DateTime date)
		{
			lock (obj) {

				var picker = new MediaPicker (this.Activity);
			
				if (!picker.IsCameraAvailable || !picker.VideosSupported)
					return;
			
				picker.TakeVideoAsync (new StoreVideoOptions
			                      {
				Name = this.generateName("movie", date),
				DesiredLength = System.TimeSpan.FromSeconds(3)
			})
				.ContinueWith (t =>
				{

					if (t.IsCanceled)
						return;

					this.Activity.RunOnUiThread (() =>
					{
						if (actualView != null) 
							actualView.HandleMovieMakerResult (t.Result.Path);
					});

//					t.Result.Dispose();
//					t.Dispose();

				});

			}
		}


	};

	class TimelineView : LinearLayout, View.IOnLongClickListener, View.IOnCreateContextMenuListener
	{
		private Timeline timeline = null;

		public Timeline Timeline {
			get {
				return timeline;
			}
		}

		private CalendarMonthView calendarView = null;


		private Fragment parent = null;
		
		public TimelineView (Timeline model, Context context, Fragment parent)
			: base(context, null)
		{
			timeline = model;

			this.parent = parent;

			View.Inflate(context, Resource.Layout.Timeline, this);
			calendarView = this.FindViewById<CalendarMonthView>(Resource.Id.calendarView2);
			calendarView.Adapter.CustomizeItem = configureItem;
			
			timeline.OnDayChanged += delegate {
				calendarView.InvalidateCalendar();
			};
			
			calendarView.OnDateSelect= (System.DateTime date) => {
				// ...
				currentDateClicked = date;

				//((TimelineFragment)this.parent).OpenGallery();
				((TimelineFragment)this.parent).TakeVideo(currentDateClicked);
			};

		}
		private System.DateTime currentDateClicked;

		public void HandleImageQueryResult (string imagePath)
		{
			timeline.AddVideoAt(currentDateClicked, imagePath);	
		}

		public void HandleMovieMakerResult (string fileResult)
		{
			timeline.AddVideoAt(currentDateClicked, fileResult);
		}
		
		private Bitmap resizeBitmap(Bitmap bm, int newHeight, int newWidth) 
		{
			int width = bm.Width;			
			int height = bm.Height;
			
			float scaleWidth = ((float) newWidth) / width;			
			float scaleHeight = ((float) newHeight) / height;
			
			// create a matrix for the manipulation			
			Matrix matrix = new Matrix();
			
			// resize the bit map			
			matrix.PostScale(scaleWidth, scaleHeight);
			
			// recreate the new Bitmap			
			return Bitmap.CreateBitmap(bm, 0, 0, width, height, matrix, false);
		}

		private Bitmap getVideoThumbnail (string filePath)
		{
			return ThumbnailUtils.CreateVideoThumbnail(filePath, ThumbnailKind.MicroKind);
		}

		private TextView configureItem (TextView view, System.DateTime date)
		{
			view.LongClickable = true;
			view.SetOnLongClickListener (this);
			view.SetOnCreateContextMenuListener (this);

			if (view.LayoutParameters.Width <= 0 || view.LayoutParameters.Height <= 0) 
				return view;

			string path = timeline.GetDayThumbnail (date);
			if (path == "") {
				return view;
			}

			//Bitmap bitmap = BitmapFactory.DecodeFile(path);
			Bitmap bitmap = getVideoThumbnail(path);
			BitmapDrawable bgImage = new BitmapDrawable(
				this.Context.Resources, resizeBitmap(bitmap, view.LayoutParameters.Width, view.LayoutParameters.Height));

			view.SetBackgroundDrawable(bgImage);

			return view;
		}

		public bool OnLongClick (View v)
		{
			v.ShowContextMenu();
			return true;
		}
		
		public void OnCreateContextMenu (IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
		{
			menu.Add(0, 0, 0, "Video");
			menu.Add(0, 1, 0, "Photo");
			menu.Add(0, 2, 0, "Options");
			
		}
	}

}

