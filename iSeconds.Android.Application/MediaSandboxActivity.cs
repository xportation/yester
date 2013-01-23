
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Media;
using iSeconds.Domain;
using Path = System.IO.Path;
using Android.Graphics;


namespace iSeconds
{
	[Activity(Label = "MediaSandboxActivity")]
	public class MediaSandboxActivity : Activity
	{
		private const string MovieDirectoryBase = "iSeconds_Movies";
		private const string PhotoDirectoryBase = "iSeconds_Photos";
		
		private string generatePictureName()
		{
			DateTime dateTime = DateTime.Now;
			string pictureName = "picture_" + dateTime.ToString() + ".jpg";
			pictureName = pictureName.Replace("/", "_");
			pictureName = pictureName.Replace(" ", "_");
			pictureName = pictureName.Replace(":", "_");
			return pictureName;
		}

		private string generateMovieName()
		{
			DateTime dateTime = DateTime.Now;
			string movieName = "movie_" + dateTime.ToString();
			movieName = movieName.Replace("/", "_");
			movieName = movieName.Replace(" ", "_");
			movieName = movieName.Replace(":", "_");
			return movieName;
		}
		
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.MediaSandbox);

			ImageView image = FindViewById<ImageView>(Resource.Id.image);
			VideoView videoView = FindViewById<VideoView>(Resource.Id.surfacevideoview);

			#region Wire up the take a video button
			Button videoButton = FindViewById<Button>(Resource.Id.takeVideoButton);
			videoButton.Click += delegate
				{
					IMediaService mediaService = new MediaServiceAndroid(this);
					string mediaFileResult = mediaService.TakeMovie(generateMovieName(), MovieDirectoryBase, 1);
					if (mediaFileResult.Length == 0)
					{
						ShowUnsupported();
						return;
					}

					RunOnUiThread(delegate
						{
							videoView.SetVideoPath(mediaFileResult);
							videoView.Start();
						});
			};
			
			#endregion

			#region Wire up the take a photo button
			Button photoButton = FindViewById<Button>(Resource.Id.takePhotoButton);
			photoButton.Click += delegate
			{				
				IMediaService mediaService = new MediaServiceAndroid(this);
				string mediaFileResult = mediaService.TakePicture(generatePictureName(), PhotoDirectoryBase);
				if (mediaFileResult.Length == 0)
				{
					ShowUnsupported();
					return;
				}

				Bitmap b = BitmapFactory.DecodeFile(mediaFileResult);
				RunOnUiThread(delegate
					{
						image.SetImageBitmap(b);
					});
			};
			#endregion

			#region Wire up the pick a video button
			Button pickVideoButton = FindViewById<Button>(Resource.Id.pickVideoButton);
			pickVideoButton.Click += delegate
			{
				IMediaService mediaService = new MediaServiceAndroid(this);
				string mediaFileResult = mediaService.PickMovie();
				if (mediaFileResult.Length == 0)
				{
					ShowUnsupported();
					return;
				}

				RunOnUiThread(delegate
				{
					videoView.SetVideoPath(mediaFileResult);
					videoView.Start();
				});
			};
			#endregion

			#region Wire up the pick a photo button
			Button pickPhotoButton = FindViewById<Button>(Resource.Id.pickPhotoButton);
			pickPhotoButton.Click += delegate
			{
				IMediaService mediaService = new MediaServiceAndroid(this);
				string mediaFileResult = mediaService.PickPicture();
				if (mediaFileResult.Length == 0)
				{
					ShowUnsupported();
					return;
				}

				Bitmap b = BitmapFactory.DecodeFile(mediaFileResult);
				RunOnUiThread(delegate
				{
					image.SetImageBitmap(b);
				});
			};
			#endregion
		}

		private Toast unsupportedToast;

		private void ShowUnsupported()
		{
			if (this.unsupportedToast != null)
			{
				this.unsupportedToast.Cancel();
				this.unsupportedToast.Dispose();
			}

			this.unsupportedToast = Toast.MakeText(this, "Your device does not support this feature", ToastLength.Long);
			this.unsupportedToast.Show();
		}
	}
}

