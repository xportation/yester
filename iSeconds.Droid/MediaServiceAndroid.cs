using System.IO;
using Android.Content;
using Android.App;
using System;
using Android.Graphics;
using Java.IO;
using iSeconds.Domain;
using Xamarin.Media;
using Android.Media;
using Android.Provider;
using File = System.IO.File;
using Stream = System.IO.Stream;

namespace iSeconds.Droid
{
    public class MediaServiceAndroid : IMediaService
    {
        private static readonly System.Object obj = new System.Object();
        private ActivityTracker activityTracker = null;
        private string mediaPath;

        public MediaServiceAndroid(ActivityTracker activityTracker, string mediaPath)
        {
            this.activityTracker = activityTracker;
            this.mediaPath = mediaPath;
        }

        public void TakeVideo(DateTime date, Action<string> resultAction)
        {
            lock (obj)
            {
                Activity currentActivity = this.activityTracker.GetCurrentActivity();
                var picker = new MediaPicker(currentActivity);

                if (!picker.IsCameraAvailable || !picker.VideosSupported)
                    return;

                picker.TakeVideoAsync(new StoreVideoOptions
                    {
                        Name = this.generateName("movie", date),
                        DesiredLength = System.TimeSpan.FromSeconds(3),
                        //Directory = this.getMediaDirectory()
                    })
                    .ContinueWith(t =>
                {

                    if (t.IsCanceled)
                        return;

                    currentActivity.RunOnUiThread(() =>
                    {
                        string path = t.Result.Path;
                        string filename = System.IO.Path.GetFileName(path);

                        string newPath = System.IO.Path.Combine(mediaPath, filename);

                        System.IO.File.Copy(t.Result.Path, newPath, true);
                        System.IO.File.Delete(t.Result.Path);

                        resultAction.Invoke(newPath);
                    });

                    //             t.Result.Dispose();
                    //             t.Dispose();

                });

            }
        }

        public void PlayVideo(string videoPath)
        {
            Activity currentActivity = this.activityTracker.GetCurrentActivity();
            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(videoPath));
            intent.SetDataAndType(Android.Net.Uri.Parse(videoPath), "video/mp4");
            currentActivity.StartActivity(intent);
        }

        public void SaveVideoThumbnail(string thumbnailPath, string videoPath)
        {
            try
            {
                Stream fileOutput = File.Create(thumbnailPath);
                Bitmap bitmap = ThumbnailUtils.CreateVideoThumbnail(videoPath, ThumbnailKind.MicroKind);
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, fileOutput);
                fileOutput.Flush();
                fileOutput.Close();
            }
            catch (Exception)
            {
            }
        }


        private string generateName(string prefix, System.DateTime dateTime)
        {
            dateTime = dateTime.Date + DateTime.Now.TimeOfDay; // setting the hour

            string movieName = prefix + "_" + dateTime.ToString();
            movieName = movieName.Replace("/", "_");
            movieName = movieName.Replace(" ", "_");
            movieName = movieName.Replace(":", "_");
            return movieName;
        }

    }
}
