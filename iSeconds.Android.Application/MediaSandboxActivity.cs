
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
		private const int RequestCode = 2;
		
		private ImageView image;
		private VideoView videoView;

		private string generateName(string prefix)
		{
			DateTime dateTime = DateTime.Now;
			string movieName = prefix + "_" + dateTime.ToString();
			movieName = movieName.Replace("/", "_");
			movieName = movieName.Replace(" ", "_");
			movieName = movieName.Replace(":", "_");
			return movieName;
		}
		
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.MediaSandbox);

			image = FindViewById<ImageView>(Resource.Id.image);
			videoView = FindViewById<VideoView>(Resource.Id.surfacevideoview);

			#region Wire up the take a video button
			Button videoButton = FindViewById<Button>(Resource.Id.takeVideoButton);
			videoButton.Click += delegate
				{
					Intent intent = MediaActivity.CreateIntentForMovie(this.generateName("movie"), 3, this);
					this.StartActivityForResult(intent, RequestCode);
				};
			
			#endregion

			#region Wire up the take a photo button
			Button photoButton = FindViewById<Button>(Resource.Id.takePhotoButton);
			photoButton.Click += delegate
				{
					Intent intent = MediaActivity.CreateIntentForPicture(this.generateName("picture"), this);
					this.StartActivityForResult(intent, RequestCode);
				};
			#endregion

			#region Wire up the pick a video button
			Button pickVideoButton = FindViewById<Button>(Resource.Id.pickVideoButton);
			pickVideoButton.Click += delegate
				{
					Intent intent = MediaActivity.CreateIntentForMovie(string.Empty, 3, this);
					this.StartActivityForResult(intent, RequestCode);
				};
			#endregion

			#region Wire up the pick a photo button
			Button pickPhotoButton = FindViewById<Button>(Resource.Id.pickPhotoButton);
			pickPhotoButton.Click += delegate
				{
					Intent intent = MediaActivity.CreateIntentForPicture(string.Empty, this);
					this.StartActivityForResult(intent, RequestCode);
				};
			#endregion
		}
		
		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == RequestCode)
			{
				if (resultCode == Result.Ok)
				{
					string fileResult = data.GetStringExtra(MediaActivity.FILE_NAME_RESULT);
					if (data.GetIntExtra(MediaActivity.MEDIA_TYPE, -1) == MediaActivity.MEDIA_TYPE_MOVIE) {
						RunOnUiThread(() =>
						{
							image.Visibility = ViewStates.Gone;
							videoView.Visibility= ViewStates.Visible;
							videoView.SetVideoPath(fileResult);

							videoView.Start();
						});
					}
					else {
						if (data.GetIntExtra(MediaActivity.MEDIA_TYPE, -1) == MediaActivity.MEDIA_TYPE_PICTURE)
						{
							Bitmap b = BitmapFactory.DecodeFile(fileResult);
							RunOnUiThread(() =>
							{
								videoView.Visibility = ViewStates.Gone;
								image.Visibility = ViewStates.Visible;

								image.SetImageBitmap(b);
							});		
						}
					}

					ShowFileResult(fileResult);
				}	
			}
		}

		private Toast fileResultToast;

		private void ShowFileResult(string fileResult)
		{
			if (this.fileResultToast != null)
			{
				this.fileResultToast.Cancel();
				this.fileResultToast.Dispose();
			}

			this.fileResultToast = Toast.MakeText(this, fileResult, ToastLength.Long);
			this.fileResultToast.Show();
		}
	}
}

