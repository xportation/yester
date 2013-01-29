using System;
using Android.App;
using Android.Content;
using Xamarin.Media;

namespace iSeconds
{
   [Activity(Label = "Media Activity")]
   public class MediaActivity : Activity
	{
		#region Static constants
		public static int TAKE_PICTURE = 0;
      public static int TAKE_MOVIE = 1;
      public static int PICK_PICTURE = 2;
      public static int PICK_MOVIE = 3;
	   
		public static string EXTRA_METHOD = "Method";
	   public static string FILE_NAME = "FileName";
	   public static string FILE_NAME_RESULT = "FileNameResult";
	   public static string TIME_IN_SECONDS = "TimeInSeconds";

	   public static string MEDIA_TYPE = "MediaType";
	   public static int MEDIA_TYPE_MOVIE = 0;
	   public static int MEDIA_TYPE_PICTURE = 1;
		#endregion

	   private bool isCanceled= true;
	   private bool isMovie= false;
	   private MediaFile mediaFileResult= null;
	   private bool alreadyStarted= false;

	   public static Intent CreateIntentForPicture(string fileName, Context context)
		{
			Intent intent = new Intent(context, typeof(MediaActivity));
			if (fileName.Length > 0)
			{
				intent.PutExtra(MediaActivity.EXTRA_METHOD, MediaActivity.TAKE_PICTURE);
				intent.PutExtra(MediaActivity.FILE_NAME, fileName);
			} 
			else
			{
				intent.PutExtra(MediaActivity.EXTRA_METHOD, MediaActivity.PICK_PICTURE);
			}

			return intent;
		}
		
		public static Intent CreateIntentForMovie(string fileName, int timeInSeconds, Context context)
		{
			Intent intent = new Intent(context, typeof(MediaActivity));
			if (fileName.Length > 0)
			{
				intent.PutExtra(MediaActivity.EXTRA_METHOD, MediaActivity.TAKE_MOVIE);
				intent.PutExtra(MediaActivity.FILE_NAME, fileName);
				intent.PutExtra(MediaActivity.TIME_IN_SECONDS, timeInSeconds);
			} 
			else
				intent.PutExtra(MediaActivity.EXTRA_METHOD, MediaActivity.PICK_MOVIE);

			return intent;
		}

		protected override void OnStart()
		{
			base.OnStart();

			if (alreadyStarted)
			{
				this.finishCurrentActivity();
				return;
			}

			alreadyStarted = true;
			Intent intent = this.Intent;
			if (!intent.HasExtra(EXTRA_METHOD))
         {
            Finish();
            return;
         }

			int methodToCall = intent.GetIntExtra(EXTRA_METHOD,-1);

			if (methodToCall == TAKE_PICTURE)
	      {
		      if (!intent.HasExtra(MediaActivity.FILE_NAME))
				{
					Finish();
					return;
				}
				
				string fileName = intent.GetStringExtra(MediaActivity.FILE_NAME);
			   takePicture(fileName);
	      }

			if (methodToCall == TAKE_MOVIE)
	      {
		      if (!intent.HasExtra(MediaActivity.FILE_NAME) || !intent.HasExtra(MediaActivity.TIME_IN_SECONDS))
				{
					Finish();
					return;
				}

				string fileName = intent.GetStringExtra(MediaActivity.FILE_NAME);
				int timeInSeconds = intent.GetIntExtra(MediaActivity.TIME_IN_SECONDS,1);
			   takeMovie(fileName,timeInSeconds);
	      }

			if (methodToCall == PICK_PICTURE)
            pickPicture();

			if (methodToCall == PICK_MOVIE)
            pickMovie();
		}
		
      private void takePicture(string pictureName)
      {
         var picker = new MediaPicker(this);

         if (!picker.IsCameraAvailable || !picker.PhotosSupported)
            return;

         picker.TakePhotoAsync(new StoreCameraMediaOptions
            {
               Name = pictureName
            })
               .ContinueWith(t =>
                  {
							xamarinMobileFinished(t.IsCanceled, false, t.Result);
							t.Dispose();
                  });
      }

      private void xamarinMobileFinished(bool isCanceled, bool isMovie, MediaFile result)
      {
	      this.isCanceled = isCanceled;
	      this.isMovie = isMovie;
	      this.mediaFileResult = result;
      }

		private void finishCurrentActivity()
		{
			Intent returnIntent = new Intent();
			if (isCanceled)
				SetResult(Result.Canceled, returnIntent);
			else
			{
				if (isMovie)
					returnIntent.PutExtra(MediaActivity.MEDIA_TYPE, MediaActivity.MEDIA_TYPE_MOVIE);
				else
					returnIntent.PutExtra(MediaActivity.MEDIA_TYPE, MediaActivity.MEDIA_TYPE_PICTURE);

				returnIntent.PutExtra(MediaActivity.FILE_NAME_RESULT, mediaFileResult.Path);
				SetResult(Result.Ok, returnIntent);
				mediaFileResult.Dispose();
			}

			Finish();
		}

		private void takeMovie(string movieName, int timeSeconds)
      {
         var picker = new MediaPicker(this);

         if (!picker.IsCameraAvailable || !picker.VideosSupported)
            return;

         picker.TakeVideoAsync(new StoreVideoOptions
            {
               Name = movieName,
               DesiredLength = TimeSpan.FromSeconds(timeSeconds)
            })
					.ContinueWith(t =>
						{
							xamarinMobileFinished(t.IsCanceled, true, t.Result);
							t.Dispose();
						});
      }

      private void pickPicture()
      {
         var picker = new MediaPicker(this);

         if (!picker.PhotosSupported)
            return;

         picker.PickPhotoAsync()
					.ContinueWith(t =>
						{
							xamarinMobileFinished(t.IsCanceled, false, t.Result);
							t.Dispose();
						});
      }

      private void pickMovie()
      {
         var picker = new MediaPicker(this);

         if (!picker.VideosSupported)
            return;

         picker.PickVideoAsync()
					.ContinueWith(t =>
						{
							xamarinMobileFinished(t.IsCanceled, true, t.Result);
							t.Dispose();
						});
      }

		public override void OnBackPressed()
		{
			isCanceled = true;
			this.finishCurrentActivity();
		}
   }
}