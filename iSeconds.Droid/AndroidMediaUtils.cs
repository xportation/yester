using System;
using System.IO;
using Android.Graphics;
using Android.Media;
using Android.Provider;

namespace iSeconds.Droid
{
	public class AndroidMediaUtils
	{
		public static void SaveVideoThumbnail(string thumbnailPath, string videoPath)
		{
			try {
				ThumbnailKind thumbnailKind = ThumbnailKind.MicroKind;
				System.IO.Stream fileOutput = File.Create(thumbnailPath);
				Bitmap bitmap = ThumbnailUtils.CreateVideoThumbnail(videoPath, thumbnailKind);
				bitmap.Compress(Bitmap.CompressFormat.Png, 100, fileOutput);
				fileOutput.Flush();
				fileOutput.Close();
			} catch(Exception) {
			}
		}
	}
}

