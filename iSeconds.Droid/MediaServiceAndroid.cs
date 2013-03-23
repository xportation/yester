using Android.Content;
using Android.App;
using System;
using iSeconds.Domain;
using Xamarin.Media;
using Android.Media;
using Android.Provider;




namespace iSeconds.Droid
{
    public class MediaServiceAndroid : IMediaService
    {
        private static readonly System.Object obj = new System.Object();

        private ActivityTracker activityTracker = null;

        public MediaServiceAndroid(ActivityTracker activityTracker)
        {
            this.activityTracker = activityTracker;         
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
                    DesiredLength = System.TimeSpan.FromSeconds(3)
                })
                .ContinueWith(t =>
                {

                    if (t.IsCanceled)
                        return;

                    currentActivity.RunOnUiThread(() =>
                    {
                        resultAction.Invoke(t.Result.Path);
                    });

                    //					t.Result.Dispose();
                    //					t.Dispose();

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

        private string generateName(string prefix, System.DateTime dateTime)
        {
            string movieName = prefix + "_" + dateTime.ToString();
            movieName = movieName.Replace("/", "_");
            movieName = movieName.Replace(" ", "_");
            movieName = movieName.Replace(":", "_");
            return movieName;
        }

    }
}
