package iSeconds.Droid;

import iSeconds.Domain.EventSource;

import java.io.IOException;
import java.util.List;
import java.util.Timer;
import java.util.TimerTask;

import android.annotation.TargetApi;
import android.app.Activity;
import android.content.Context;
import android.hardware.Camera;
import android.media.CamcorderProfile;
import android.media.MediaRecorder;
import android.media.MediaRecorder.AudioSource;
import android.media.MediaRecorder.VideoSource;
import android.os.Build;
import android.provider.MediaStore.Audio;
import android.util.AttributeSet;
import android.util.Log;
import android.view.Display;
import android.view.Surface;
import android.view.SurfaceHolder;
import android.view.SurfaceView;
import android.view.ViewGroup;
import android.widget.Toast;

public class CamcorderView extends SurfaceView implements
		SurfaceHolder.Callback, MediaRecorder.OnInfoListener {

	private Activity activity;
	private SurfaceHolder holder;
	private Camera camera;
	private List<Camera.Size> previewSizeList;
	private List<Camera.Size> pictureSizeList;
	private Camera.Size previewSize;
	private Camera.Size pictureSize;
	private int cameraId;
	private int cameraAngle = 0;
	private MediaRecorder mediaRecorder;
	private static final String LogYesterCamcorder = "Yester Camcorder";

	public EventSource onCameraReady = new EventSource();
	public EventSource onVideoRecorded = new EventSource();
	public EventSource onSecondChange = new EventSource();

	private Timer timer = null;
	private SecondsTimerTask secondsTimerTask = null;

	public CamcorderView(Activity activity, int cameraId) {

		super(activity);
		this.activity = activity;

		holder = this.getHolder();
		holder.addCallback(this);
		holder.setType(SurfaceHolder.SURFACE_TYPE_PUSH_BUFFERS);

		this.cameraId = cameraId;
	}

	@TargetApi(Build.VERSION_CODES.GINGERBREAD)
	public void initializeCamera(int cameraId) {

		if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.GINGERBREAD) {
			if (Camera.getNumberOfCameras() > cameraId) // TODO: ver essa logica
													// cameraId
				this.cameraId = cameraId;
			else
				this.cameraId = 0;
		} else
			this.cameraId = 0;

		if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.GINGERBREAD)
			this.camera = Camera.open(this.cameraId);
		else
			this.camera = Camera.open();

		Camera.Parameters cameraParams = camera.getParameters();
		previewSizeList = cameraParams.getSupportedPreviewSizes();
		pictureSizeList = cameraParams.getSupportedPictureSizes();

		onCameraReady.trigger(this, null);
	}

	public List<Camera.Size> getCameraSizes() {
		return this.previewSizeList;
	}

	@Override
	public void surfaceChanged(SurfaceHolder holder, int format, int width,
			int height) {
		if (!changeSurfaceSize(width, height))
			Toast.makeText(activity, "Can't start preview", Toast.LENGTH_LONG)
					.show();
	}

	@Override
	public void surfaceCreated(SurfaceHolder holder) {
		try {
			initializeCamera(cameraId);
			camera.setPreviewDisplay(holder);
		} catch (IOException e) {
			camera.release();
			camera = null;
		}
	}

	@Override
	public void surfaceDestroyed(SurfaceHolder holder) {
		if (null == camera)
			return;

		camera.stopPreview();
		camera.release();
		camera = null;
	}

	private boolean changeSurfaceSize(int width, int height) {
		camera.stopPreview();

		if (previewSizeList.size() == 0)
			return false;

		boolean cameraSizeOk = false;
		Camera.Parameters cameraParameters = camera.getParameters();
		boolean portrait = isPortrait();
		while (!cameraSizeOk) {
			this.previewSize = determinePreviewSize(portrait, width, height);
			this.pictureSize = determinePictureSize(previewSize);
			adjustSurfaceLayoutSize(previewSize, portrait, width, height);
			configureCameraParameters(cameraParameters, portrait);

			try {
				camera.startPreview();
				cameraSizeOk = true;
				break;
			} catch (Exception e) {
				Log.w(LogYesterCamcorder,
						"Failed to start preview: " + e.getMessage());

				// Remove failed size
				previewSizeList.remove(this.previewSize);
				previewSize = null;

				if (previewSizeList.size() == 0) {
					Log.w(LogYesterCamcorder, "Gave up starting preview");
					break;
				}
			}
		}

		return cameraSizeOk;
	}

	public boolean isPortrait() {
		return activity.getResources().getConfiguration().orientation == android.content.res.Configuration.ORIENTATION_PORTRAIT;
	}

	private Camera.Size determinePreviewSize(boolean portrait, int width,
			int height) {
		int previewWidth = width;
		int previewHeight = height;
		if (portrait) {
			previewWidth = height;
			previewHeight = width;
		}
		return determineSize(previewSizeList, previewWidth, previewHeight);
	}

	private Camera.Size determinePictureSize(Camera.Size previewSize) {

		for (Camera.Size size : pictureSizeList) {
			if (size.equals(previewSize)) {
				return size;
			}
		}

		return determineSize(pictureSizeList, previewSize.width,
				previewSize.height);
	}

	private Camera.Size determineSize(List<Camera.Size> sizes,
			int previewWidth, int previewHeight) {

		float deltaRatioMin = Float.MAX_VALUE;
		float ratio = ((float) previewWidth) / previewHeight;

		Camera.Size retangleSize = null;
		for (Camera.Size size : sizes) {
			float currentRatio = ((float) size.width) / size.height;
			float deltaRatio = Math.abs(ratio - currentRatio);
			if (deltaRatio < deltaRatioMin) {
				deltaRatioMin = deltaRatio;
				retangleSize = size;
			}
		}

		return retangleSize;
	}

	private void adjustSurfaceLayoutSize(Camera.Size previewSize,
			boolean portrait, int availableWidth, int availableHeight) {
		float tmpLayoutHeight = previewSize.height;
		float tmpLayoutWidth = previewSize.width;
		if (portrait) {
			tmpLayoutHeight = previewSize.width;
			tmpLayoutWidth = previewSize.height;
		}

		float factH = availableHeight / tmpLayoutHeight;
		float factW = availableWidth / tmpLayoutWidth;
		float fact = (factH < factW) ? factH : factW;

		ViewGroup.LayoutParams layoutParams = this.getLayoutParams();
		int layoutHeight = (int) (tmpLayoutHeight * fact);
		int layoutWidth = (int) (tmpLayoutWidth * fact);

		if ((layoutWidth != this.getWidth())
				|| (layoutHeight != this.getHeight())) {
			layoutParams.height = layoutHeight;
			layoutParams.width = layoutWidth;
			this.setLayoutParams(layoutParams);
		}
	}

	protected void configureCameraParameters(
			Camera.Parameters cameraParameters, boolean portrait) {

		if (android.os.Build.VERSION.SDK_INT < android.os.Build.VERSION_CODES.FROYO) { // for
																						// 2.1
																						// and
																						// before
			if (portrait) {
				cameraParameters.set("orientation", "portrait");
			} else {
				cameraParameters.set("orientation", "landscape");
			}
		} else { // for 2.2 and later
			int angle;
			Display display = activity.getWindowManager().getDefaultDisplay();
			switch (display.getRotation()) {
			case Surface.ROTATION_0:
				angle = 90;
				break;
			case Surface.ROTATION_90:
				angle = 0;
				break;
			case Surface.ROTATION_180:
				angle = 270;
				break;
			case Surface.ROTATION_270:
				angle = 180;
				break;
			default:
				angle = 90;
				break;
			}
			Log.v(LogYesterCamcorder, "angle: " + angle);
			this.cameraAngle = angle;
			camera.setDisplayOrientation(angle);
		}

		cameraParameters.setPreviewSize(previewSize.width, previewSize.height);
		cameraParameters.setPictureSize(pictureSize.width, pictureSize.height);

		camera.setParameters(cameraParameters);
	}

	public void SetPreviewSize(int index, int width, int height) {
		camera.stopPreview();
		Camera.Parameters cameraParameters = camera.getParameters();
		boolean portrait = isPortrait();
		this.previewSize = previewSizeList.get(index);
		this.pictureSize = determinePictureSize(this.previewSize);

		adjustSurfaceLayoutSize(this.previewSize, portrait, width, height);
		configureCameraParameters(cameraParameters, portrait);

		try {
			camera.startPreview();
		} catch (Exception e) {
			Log.w(LogYesterCamcorder,
					"Failed to start preview: " + e.getMessage());
		}
	}

	private boolean prepareMediaRecorder(String filename, int duration) {

		initializeCamera(this.cameraId);
		mediaRecorder = new MediaRecorder();

		mediaRecorder.setOnInfoListener(this);
		
		Camera.Parameters cameraParameters = camera.getParameters();
		this.configureCameraParameters(cameraParameters, isPortrait());

//		camera.setDisplayOrientation(this.cameraAngle);
		camera.unlock();
		
		mediaRecorder.setCamera(camera);

		mediaRecorder.setAudioSource(AudioSource.CAMCORDER);
		mediaRecorder.setVideoSource(VideoSource.CAMERA);

		mediaRecorder.setProfile(CamcorderProfile
				.get(CamcorderProfile.QUALITY_LOW));

		mediaRecorder.setOutputFile(filename);
		mediaRecorder.setMaxDuration(duration); // Set max duration 60 sec.
		// mediaRecorder.SetMaxFileSize(5000000); // Set max file size 5M

		mediaRecorder.setPreviewDisplay(holder.getSurface());

		try {
			mediaRecorder.prepare();
		} catch (IllegalStateException e) {
			releaseMediaRecorder();
			return false;
		} catch (IOException e) {
			releaseMediaRecorder();
			return false;
		}
		return true;

	}

	public void startRecording(String filename, int duration) {

		// Release Camera before MediaRecorder start
		releaseCamera();

		if (!prepareMediaRecorder(filename, duration)) {
			return;
		}

		if (timer != null)
			timer.cancel();

		timer = new Timer();
		secondsTimerTask = new SecondsTimerTask(this);

		timer.schedule(secondsTimerTask, 1000, 1000);

		mediaRecorder.start();
	}

	private void releaseMediaRecorder() {
		if (mediaRecorder != null) {
			mediaRecorder.reset(); // clear recorder configuration
			mediaRecorder.release(); // release the recorder object
			mediaRecorder = null;
			camera.lock(); // lock camera for later use
		}
	}

	private void releaseCamera() {
		if (camera != null) {
			camera.release(); // release the camera for other applications
			camera = null;
		}
	}

	public void StopRecording() {
		if (mediaRecorder != null) {
			camera.stopPreview();
			mediaRecorder.stop();
			mediaRecorder.release();
		}
	}
	
	public void stopCamcorder() {
//		this.releaseMediaRecorder();
//		this.releaseCamera();
		
//		if (camera != null) {
//			camera.stopPreview();
//			camera.release();
//			camera = null;
//		}
		
//		if (mediaRecorder != null) {
//			mediaRecorder.stop();
//			mediaRecorder.release();
//			mediaRecorder = null;
//		}
		this.setVisibility(INVISIBLE);
//		if (timer != null) {
//			timer.cancel();
//			timer = null;
//		}
		
	}

	public void startCamcorder() {
		this.setVisibility(VISIBLE);
		
	}

	@Override
	public void onInfo(MediaRecorder mr, int what, int extra) {

		if (what == MediaRecorder.MEDIA_RECORDER_INFO_MAX_DURATION_REACHED) {
			
			Log.i("YESTER", "record stoped");
			
//			stopCamcorder();

			this.releaseMediaRecorder();
			this.releaseCamera();

			timer.cancel();
			timer = null;

			// if (onVideoRecorded != null)
			onVideoRecorded.trigger(this, null);
		}

	}

	private class SecondsTimerTask extends TimerTask {
		private int seconds = 0;
		private CamcorderView view;

		public SecondsTimerTask(CamcorderView view) {
			this.view = view;
		}

		@Override
		public void run() {
			seconds++;
			Log.i("YESTER", "seconds " + seconds);
			view.onSecondChange.trigger(this, "" + seconds);
		}
	}

}
