using System;
using Android.Content;
using Xamarin.Media;
using iSeconds.Domain;

namespace iSeconds
{
	internal class MediaServiceAndroid : IMediaService
	{
		private readonly Context context;

		public MediaServiceAndroid(Context context)
		{
			this.context = context;
		}

		public string TakePicture(string pictureName, string pictureDirectory)
		{
			string picturePathResult = string.Empty;
			var picker = new MediaPicker(context);

			if (!picker.IsCameraAvailable || !picker.PhotosSupported)
				return picturePathResult;

			picker.TakePhotoAsync(new StoreCameraMediaOptions
				{
					Name = pictureName,
					Directory = pictureDirectory
				})
				.ContinueWith(t =>
					{
						if (t.IsCanceled)
							return picturePathResult;

						picturePathResult = t.Result.Path;
						t.Result.Dispose();
						return picturePathResult;
					});

			return picturePathResult;
		}

		public string TakeMovie(string movieName, string movieDirectory, int timeSeconds)
		{
			string moviePathResult = string.Empty;
			var picker = new MediaPicker(context);

			if (!picker.IsCameraAvailable || !picker.VideosSupported)
				return moviePathResult;

			picker.TakeVideoAsync(new StoreVideoOptions
				{
					Name = movieName,
					Directory = movieDirectory,
					DesiredLength = TimeSpan.FromSeconds(timeSeconds)
				})
				.ContinueWith(t =>
					{
						if (t.IsCanceled)
							return moviePathResult;

						moviePathResult = t.Result.Path;
						t.Result.Dispose();
						return moviePathResult;
					});

			return moviePathResult;
		}

		//TODO [leonardo] copiar para um caminho pre-definido???
		public string PickPicture()
		{
			string picturePathResult = string.Empty;
			var picker = new MediaPicker(context);

			if (!picker.PhotosSupported)
				return picturePathResult;

			picker.PickPhotoAsync().ContinueWith(t =>
				{
					if (t.IsCanceled)
						return picturePathResult;

					picturePathResult = t.Result.Path;
					t.Result.Dispose();
					return picturePathResult;
				});

			return picturePathResult;
		}

		//TODO [leonardo] copiar para um caminho pre-definido???
		public string PickMovie()
		{
			string moviePathResult = string.Empty;
			var picker = new MediaPicker(context);

			if (!picker.VideosSupported)
				return moviePathResult;

			picker.PickVideoAsync().ContinueWith(t =>
				{
					if (t.IsCanceled)
						return moviePathResult;

					moviePathResult = t.Result.Path;
					t.Result.Dispose();
					return moviePathResult;
				});

			return moviePathResult;
		}
	}
}