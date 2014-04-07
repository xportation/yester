using System;
using Android.App;
using Android.Views;
using Android.Hardware;
using System.Collections.Generic;
using Android.Widget;
using Android.Util;
using Android.Media;
using System.Diagnostics;
using Java.Lang;
using Java.IO;
using Java.Util;
using iSeconds.Domain;

namespace iSeconds.Droid
{
		public class CamcorderView : SurfaceView, ISurfaceHolderCallback, MediaRecorder.IOnInfoListener
		{
				private Activity activity;
				private ISurfaceHolder holder;
				private Camera camera;
				private IList<Camera.Size> previewSizeList;
				private IList<Camera.Size> pictureSizeList;
				private Camera.Size previewSize;
				private Camera.Size pictureSize;
				private int cameraId;
				private int cameraAngle = 0;
				private MediaRecorder mediaRecorder;
				private const string LogYesterCamcorder = "Yester Camcorder";

				public event EventHandler OnCameraReady;
				public event EventHandler OnVideoRecorded;
				public event EventHandler OnSecondChange;

				private Timer timer = null;
				private SecondsTimerTask secondsTimerTask = null;

				public CamcorderView (Activity activity, int cameraId)
			:	base (activity)
				{
						this.activity = activity;
						holder = this.Holder;
						holder.AddCallback (this);
						holder.SetType (SurfaceType.PushBuffers);

						this.cameraId = cameraId;
				}

				public void initializeCamera (int cameraId)
				{
						if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Gingerbread) {
								if (Camera.NumberOfCameras > cameraId) // TODO: ver essa logica cameraId
										this.cameraId = cameraId;
								else
										this.cameraId = 0;
						} else
								this.cameraId = 0;

						if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Gingerbread)
								this.camera = Camera.Open (this.cameraId);
						else
								this.camera = Camera.Open ();

						Camera.Parameters cameraParams = camera.GetParameters ();
						previewSizeList = cameraParams.SupportedPreviewSizes;
						pictureSizeList = cameraParams.SupportedPictureSizes;

						OnCameraReady.Invoke (this, null);
				}

			public IList<Camera.Size> CameraSizes {
					get	{
						return this.previewSizeList;
					}
			}

				public void SurfaceChanged (ISurfaceHolder holder, Android.Graphics.Format format, int width, int height)
				{
						if (!changeSurfaceSize (width, height))
								Toast.MakeText (activity, "Can't start preview", ToastLength.Long).Show ();
				}

				public void SurfaceCreated (ISurfaceHolder holder)
				{
						try {
								initializeCamera (cameraId);
								camera.SetPreviewDisplay (holder);
						} catch (Java.IO.IOException) {
								camera.Release ();
								camera = null;
						}
				}

				public void SurfaceDestroyed (ISurfaceHolder holder)
				{
						if (null == camera)
								return;

						camera.StopPreview ();
						camera.Release ();
						camera = null;
				}

				private bool changeSurfaceSize (int width, int height)
				{
						camera.StopPreview ();

						if (previewSizeList.Count == 0)
								return false;

						bool cameraSizeOk = false;
						Camera.Parameters cameraParameters = camera.GetParameters ();
						bool portrait = activity.Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait;
						while (!cameraSizeOk) {
								this.previewSize = determinePreviewSize (portrait, width, height);
								this.pictureSize = determinePictureSize (previewSize);
								adjustSurfaceLayoutSize (previewSize, portrait, width, height);
								configureCameraParameters (cameraParameters, portrait);

								try {
										camera.StartPreview ();
										cameraSizeOk = true;
										break;
								} catch (Java.Lang.Exception e) {
										Log.Warn (LogYesterCamcorder, "Failed to start preview: " + e.Message);

										// Remove failed size
										previewSizeList.Remove (this.previewSize);
										previewSize = null;

										if (previewSizeList.Count == 0) {
												Log.Warn (LogYesterCamcorder, "Gave up starting preview");
												break;
										}
								}
						}

						return cameraSizeOk;
				}

				private Camera.Size determinePreviewSize (bool portrait, int width, int height)
				{
						int previewWidth = width;
						int previewHeight = height;
						if (portrait) {
								previewWidth = height;
								previewHeight = width;
						} 
						return determineSize (previewSizeList, previewWidth, previewHeight);
				}

				private Camera.Size determinePictureSize (Camera.Size previewSize)
				{
						foreach (Camera.Size size in pictureSizeList) {
								if (size.Equals (previewSize)) {
										return size;
								}
						}

						return determineSize (pictureSizeList, previewSize.Width, previewSize.Height);
				}

				private Camera.Size determineSize (IList<Camera.Size> sizes, int previewWidth, int previewHeight)
				{
						float deltaRatioMin = float.MaxValue;
						float ratio = ((float)previewWidth) / previewHeight;

						Camera.Size retangleSize = null;
						foreach (Camera.Size size in sizes) {
								float currentRatio = ((float)size.Width) / size.Height;
								float deltaRatio = System.Math.Abs (ratio - currentRatio);
								if (deltaRatio < deltaRatioMin) {
										deltaRatioMin = deltaRatio;
										retangleSize = size;
								}
						}

						return retangleSize;
				}

				private void adjustSurfaceLayoutSize (Camera.Size previewSize, bool portrait, int availableWidth, int availableHeight)
				{
						float tmpLayoutHeight = previewSize.Height;
						float tmpLayoutWidth = previewSize.Width;
						if (portrait) {
								tmpLayoutHeight = previewSize.Width;
								tmpLayoutWidth = previewSize.Height;
						}

						float factH = availableHeight / tmpLayoutHeight;
						float factW = availableWidth / tmpLayoutWidth;
						float fact = (factH < factW) ? factH : factW;

						ViewGroup.LayoutParams layoutParams = this.LayoutParameters;
						int layoutHeight = (int)(tmpLayoutHeight * fact);
						int layoutWidth = (int)(tmpLayoutWidth * fact);

						if ((layoutWidth != this.Width) || (layoutHeight != this.Height)) {
								layoutParams.Height = layoutHeight;
								layoutParams.Width = layoutWidth;
								this.LayoutParameters = layoutParams;
						}
				}

				protected void configureCameraParameters (Camera.Parameters cameraParameters, bool portrait)
				{
						if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Froyo) { // for 2.1 and before
								if (portrait) {
										cameraParameters.Set ("orientation", "portrait");
								} else {
										cameraParameters.Set ("orientation", "landscape");
								}
						} else { // for 2.2 and later
								int angle;
								Display display = activity.WindowManager.DefaultDisplay;
								switch (display.Rotation) {
								case SurfaceOrientation.Rotation0:
										angle = 90; 
										break;
								case SurfaceOrientation.Rotation90:
										angle = 0;
										break;
								case SurfaceOrientation.Rotation180:
										angle = 270;
										break;
								case SurfaceOrientation.Rotation270:
										angle = 180;
										break;
								default:
										angle = 90;
										break;
								}
								Log.Verbose (LogYesterCamcorder, "angle: " + angle);
								this.cameraAngle = angle;
								camera.SetDisplayOrientation (angle);
						}

						cameraParameters.SetPreviewSize (previewSize.Width, previewSize.Height);
						cameraParameters.SetPictureSize (pictureSize.Width, pictureSize.Height);

						camera.SetParameters (cameraParameters);
				}

			public void SetPreviewSize (int index, int width, int height)
			{
				 camera.StopPreview ();
				 Camera.Parameters cameraParameters = camera.GetParameters();
				 bool portrait = activity.Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait;
				 this.previewSize = previewSizeList[index];
				 this.pictureSize = determinePictureSize(this.previewSize);

				 adjustSurfaceLayoutSize(this.previewSize, portrait, width, height);
				 configureCameraParameters (cameraParameters, portrait);

				 try {
						camera.StartPreview ();
				 } catch (Java.Lang.Exception e) {
						Log.Warn (LogYesterCamcorder, "Failed to start preview: " + e.Message);
				 }          
			}

			private bool prepareMediaRecorder(string filename, int duration) {

				 initializeCamera (this.cameraId);
				 mediaRecorder = new MediaRecorder();

				 mediaRecorder.SetOnInfoListener (this);

				 camera.Unlock();
				 mediaRecorder.SetCamera(camera);

				 mediaRecorder.SetAudioSource(AudioSource.Camcorder);
				 mediaRecorder.SetVideoSource(VideoSource.Camera);

				 mediaRecorder.SetProfile (CamcorderProfile.Get(CamcorderQuality.Low));

				 mediaRecorder.SetOutputFile(filename);
				 mediaRecorder.SetMaxDuration(duration); // Set max duration 60 sec.
				 //mediaRecorder.SetMaxFileSize(5000000); // Set max file size 5M

				 mediaRecorder.SetPreviewDisplay(Holder.Surface);

				 try {
						mediaRecorder.Prepare();
				 } catch (IllegalStateException) {
						releaseMediaRecorder();
						return false;
				 } catch (IOException) {
						releaseMediaRecorder();
						return false;
				 }
				 return true;

			}

			public void StartRecording (string filename, int duration)
			{

				// Release Camera before MediaRecorder start
				releaseCamera();

				if (!prepareMediaRecorder(filename, duration)) {
					return;
				}

				if (timer != null)
					timer.Cancel ();

				timer = new Timer ();
				secondsTimerTask = new SecondsTimerTask (this);
				
				timer.Schedule (secondsTimerTask, 1000, 1000);

				mediaRecorder.Start();
			}

			private void releaseMediaRecorder() {
				 if (mediaRecorder != null) {
						mediaRecorder.Reset(); // clear recorder configuration
						mediaRecorder.Release(); // release the recorder object
						mediaRecorder = null;
						camera.Lock(); // lock camera for later use
				 }
			}

			private void releaseCamera() {
				 if (camera != null) {
						camera.Release(); // release the camera for other applications
						camera = null;
				 }
			}

			public void StopRecording ()
			{
				if (mediaRecorder != null) {
					camera.StopPreview ();
					mediaRecorder.Stop ();
					mediaRecorder.Release ();
				}
			}
				
			public void OnInfo (MediaRecorder mr, MediaRecorderInfo what, int extra)
			{
				if (what == MediaRecorderInfo.MaxDurationReached) {

					this.releaseMediaRecorder ();
					this.releaseCamera ();

					timer.Cancel ();
					timer = null;

					if (OnVideoRecorded != null)
						OnVideoRecorded.Invoke (this, null);
				}


			}

			class SecondsTimerTask : TimerTask
			{
				private int seconds = 0;
				private CamcorderView view;

				public SecondsTimerTask(CamcorderView view)
				{
					this.view = view;
				}

				public override void Run() 
				{
					seconds++;
					System.Console.WriteLine ("seconds " + seconds);
					view.OnSecondChange.Invoke (this, new GenericEventArgs<int>(seconds));
				}
			}
		}
}

