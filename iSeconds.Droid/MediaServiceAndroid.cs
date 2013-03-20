using Android.Content;
using Android.App;
using System;
using iSeconds.Domain;
using Xamarin.Media;


namespace iSeconds.Droid
{
    public class MediaServiceAndroid : IMediaService
    {
        private static readonly System.Object obj = new System.Object();

        private Activity context = null;

        public MediaServiceAndroid(Activity context)
        {
            this.context = context;
        }

        public void TakeVideo(DateTime date, Action<string> resultAction)
        {
            lock (obj)
            {
                var picker = new MediaPicker(this.context);

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

                    this.context.RunOnUiThread(() =>
                    {
                        resultAction.Invoke(t.Result.Path);
                    });

                    //					t.Result.Dispose();
                    //					t.Dispose();

                });

            }
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
