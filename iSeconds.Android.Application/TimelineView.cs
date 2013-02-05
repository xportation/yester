
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
			

		
		public override void OnActivityResult(int requestCode, int resultCode, Intent data) 
		{
			if (requestCode == ISecondsConstants.SELECT_PHOTO_RESULT && data != null && data.Data != null)
			{
				Uri _uri = data.Data;
				
				if (_uri != null) 
				{
					
					//User had pick an image.
					var cursor = this.Activity.ContentResolver.Query(
						_uri, new string[] { MediaStore.Images.ImageColumns.Data }, null, null, null
					);
					cursor.MoveToFirst();
					
					//Link to the image
					string imageFilePath = cursor.GetString(0);
					cursor.Close();

					if (actualView != null)
						actualView.HandleImageQueryResult(imageFilePath);
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


	};

	class TimelineView : LinearLayout, GestureDetector.IOnGestureListener
	{
		private Timeline timeline = null;

		public Timeline Timeline {
			get {
				return timeline;
			}
		}

		public override bool OnInterceptTouchEvent (MotionEvent ev)
		{
			gestureDetector.OnTouchEvent(ev);
			return base.OnInterceptTouchEvent (ev);		
		}

		public bool OnDown (MotionEvent e)
		{
			return true;
		}
		
		public bool OnFling (MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
		{
			if (e1.GetX() > e2.GetX()) // right to left
				calendarView.DecrementMonth();
			else
				calendarView.IncrementMonth();

			return true;
		}
		
		public void OnLongPress (MotionEvent e)
		{
		}
		
		public bool OnScroll (MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
		{
			return true;
		}
		
		public void OnShowPress (MotionEvent e)
		{
		}
		
		public bool OnSingleTapUp (MotionEvent e)
		{
			return true;
		}	



		private CalendarMonthView calendarView = null;
		private GestureDetector gestureDetector;

		private Fragment parent = null;
		
		public TimelineView (Timeline model, Context context, Fragment parent)
			: base(context, null)
		{
			timeline = model;

			this.parent = parent;

			gestureDetector = new GestureDetector(this);

			View.Inflate(context, Resource.Layout.Timeline, this);
			calendarView = this.FindViewById<CalendarMonthView>(Resource.Id.calendarView2);
			calendarView.Adapter.CustomizeItem = configureItem;
			
			timeline.OnDayChanged += delegate {
				calendarView.InvalidateCalendar();
			};
			
			calendarView.OnDateSelect= (System.DateTime date) => {
				// ...
				currentDateClicked = date;
				((TimelineFragment)this.parent).OpenGallery();
			};

		}
		private System.DateTime currentDateClicked;

		public void HandleImageQueryResult (string imagePath)
		{
			timeline.AddVideoAt(currentDateClicked, imagePath);	
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

		private TextView configureItem(TextView view, System.DateTime date)
		{
			if (view.Width <= 0 || view.Height <= 0) 
				return view;

			string path = timeline.GetDayThumbnail(date);
			if (path == "")
				return view;

			System.Console.WriteLine(date.ToString());
			System.Console.WriteLine(path);

			Bitmap bitmap = BitmapFactory.DecodeFile(path);
			BitmapDrawable bgImage = new BitmapDrawable(
				this.Context.Resources, resizeBitmap(bitmap, view.Width, view.Height));

			view.SetBackgroundDrawable(bgImage);

			return view;
		}
	}
}

