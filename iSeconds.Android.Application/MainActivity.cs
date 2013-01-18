﻿using System;
using System.IO;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Widget;
using Android.OS;
using iSeconds.Domain;
using Xamarin.Media;
using Path = System.IO.Path;
using Android.Content;

namespace iSeconds.Android.Application
{
	[Activity (Label = "iSeconds", MainLauncher = true, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.Orientation)]
	public class MainActivity : Activity
	{
		// um usuario de teste
		User userTest = new User();

		int CREATE_TIMELINE_RESULT = 1;

		const string TIMELINE_NAME_EXTRA = "TIMELINE_NAME_EXTRA";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			Button createTimelineButton = FindViewById<Button>(Resource.Id.createTimeline);
			createTimelineButton.Click += delegate {
				this.StartActivityForResult(typeof(CreateTimelineActivity), CREATE_TIMELINE_RESULT);
			};
				
			ImageView image = FindViewById<ImageView> (Resource.Id.image);
			VideoView videoView  = FindViewById<VideoView>(Resource.Id.surfacevideoview);
			
			
			//
			// Wire up the take a video button
			//
			Button videoButton = FindViewById<Button> (Resource.Id.takeVideoButton);
			videoButton.Click += delegate
			{
				//
				// The MediaPicker is the class used to 
				// invoke the camera and gallery picker
				// for selecting and taking photos
				// and videos
				//
				var picker = new MediaPicker (this);



				// We can check to make sure the device has a camera
				// and supports dealing with video.
				if (!picker.IsCameraAvailable || !picker.VideosSupported)
				{
					ShowUnsupported();
					return;
				}
				
				//
				// TakeVideoAsync is an async API that takes a 
				// StoreVideoOptions object with various 
				// properties, such as the name and folder to
				// store the resulting video. You can
				// also limit the length of the video
				//
				picker.TakeVideoAsync (new StoreVideoOptions
				{
					Name = "MyVideo",
					Directory = "MyVideos",
					DesiredLength = TimeSpan.FromSeconds (10)
				})
				.ContinueWith (t =>
				{
					if (t.IsCanceled)
						return;

					//
					// Because TakeVideoAsync returns a Task
					// we can use ContinueWith to run more code
					// after the user finishes recording the video
					//
					RunOnUiThread (delegate
					{
						//
						// Toggle the visibility of the image and videoviews
						//
						image.Visibility = Android.Views.ViewStates.Gone;	
						videoView.Visibility = Android.Views.ViewStates.Visible;
						
						//	
						// Load in the video file
						//
						videoView.SetVideoPath(t.Result.Path);
	        
						//
						// optional: Handle when the video finishes playing
						//
	        			//videoView.setOnCompletionListener(this);
	        
						//
						// Start playing the video
						//	
	        			videoView.Start();
					});
				});
			};
			
			//
			// Wire up the take a photo button
			//
			Button photoButton = FindViewById<Button> (Resource.Id.takePhotoButton);
			photoButton.Click += delegate
			{
				var picker = new MediaPicker (this);

				if (!picker.IsCameraAvailable || !picker.PhotosSupported)
				{
					ShowUnsupported();
					return;
				}

				picker.TakePhotoAsync (new StoreCameraMediaOptions
				{
					Name = "test.jpg",
					Directory = "MediaPickerSample"
				})
				.ContinueWith (t =>
				{
					if (t.IsCanceled)
						return;

					Bitmap b = BitmapFactory.DecodeFile (t.Result.Path);
					RunOnUiThread (delegate
					{
						//
						// Toggle the visibility of the image and video views
						//
						videoView.Visibility = Android.Views.ViewStates.Gone;
						image.Visibility = Android.Views.ViewStates.Visible;
							
						//
						// Display the bitmap
						//
						image.SetImageBitmap (b);

						// Cleanup any resources held by the MediaFile instance
						t.Result.Dispose();
					});
				});
			};
			
			//
			// Wire up the pick a video button
			//
			Button pickVideoButton = FindViewById<Button> (Resource.Id.pickVideoButton);
			pickVideoButton.Click += delegate
			{
				//
				// The MediaPicker is the class used to 
				// invoke the camera and gallery picker
				// for selecting and taking photos
				// and videos
				//
				var picker = new MediaPicker (this);
				
				if (!picker.VideosSupported)
				{
					ShowUnsupported();
					return;
				}

				//
				// PickVideoAsync is an async API that invokes
				// the native gallery
				//
				picker.PickVideoAsync ()
				.ContinueWith (t =>
				{
					if (t.IsCanceled)
						return;

					//
					// Because PickVideoAsync returns a Task
					// we can use ContinueWith to run more code
					// after the user finishes recording the video
					//
					RunOnUiThread (delegate
					{
						//
						// Toggle the visibility of the image and video views
						//
						image.Visibility = Android.Views.ViewStates.Gone;	
						videoView.Visibility = Android.Views.ViewStates.Visible;
						
						//	
						// Load in the video file
						//
						videoView.SetVideoPath(t.Result.Path);
	        
						//
						// Optional: Handle when the video finishes playing
						//
	        			//videoView.setOnCompletionListener(this);
	        
						//
						// Start playing the video
						//	
	        			videoView.Start();

						// Cleanup any resources held by the MediaFile instance
						t.Result.Dispose();
					});
				});
			};
			
			//
			// Wire up the pick a photo button
			//
			Button pickPhotoButton = FindViewById<Button> (Resource.Id.pickPhotoButton);
			pickPhotoButton.Click += delegate
			{
				var picker = new MediaPicker (this);

				if (!picker.PhotosSupported)
				{
					ShowUnsupported();
					return;
				}

				picker.PickPhotoAsync ()
				.ContinueWith (t =>
				{
					if (t.IsCanceled)
						return;

					Bitmap b = BitmapFactory.DecodeFile (t.Result.Path);
					RunOnUiThread (delegate
					{
						//
						// Toggle the visibility of the image and video views
						//
						videoView.Visibility = Android.Views.ViewStates.Gone;
						image.Visibility = Android.Views.ViewStates.Visible;
							
						//
						// Display the bitmap
						//
						image.SetImageBitmap (b);

						// Cleanup any resources held by the MediaFile instance
						t.Result.Dispose();
					});
				});
			};
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == CREATE_TIMELINE_RESULT) 
			{
				if (resultCode == Result.Ok) 
				{
					string timelineName = data.GetStringExtra(TIMELINE_NAME_EXTRA);
					Toast toast = Toast.MakeText(this, timelineName, ToastLength.Short);
					toast.Show();
					// TODO: parei aqui
					//userTest.CreateTimeline(timelineName);
				}

			}
		}

		private Toast unsupportedToast;
		private void ShowUnsupported()
		{
			if (this.unsupportedToast != null)
			{
				this.unsupportedToast.Cancel();
				this.unsupportedToast.Dispose();
			}
			
			this.unsupportedToast = Toast.MakeText (this, "Your device does not support this feature", ToastLength.Long);
			this.unsupportedToast.Show();
		}
	
	}

		
}