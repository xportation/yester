package iSeconds.Droid;

import iSeconds.Domain.EventSource.EventSourceListener;
import iSeconds.Domain.ISecondsUtils;
import iSeconds.Domain.User;

import java.io.File;
import java.util.Date;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ImageButton;
import android.widget.RadioGroup;
import android.widget.RelativeLayout;
import android.widget.Spinner;
import android.widget.TextView;

public class CamcorderFragment extends Fragment {
	
	boolean isRecording = false;
	CamcorderView camcorderView = null;
	String videoPath;
	String thumbnailPath;
	Date videoDate;

	// private IMediaService mediaService = null;
	// private UserService userService = null;

	static final int PREVIEW_RESULT = 1;

	private TextView countDownText = null;
	private RadioGroup durationGroup = null;

	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		
		super.onCreateView(inflater, container, savedInstanceState);
		
//		setContentView(R.layout.camcorder);
		
		View rootView = inflater.inflate(R.layout.camcorder,
				container, false);

		configureServicesAndParameters();
		configureCamcorderView(rootView);
		configureRecordButton(rootView);
		configureResolutionDropdown(rootView);
		configureCountdown(rootView);
		configureDurationGroup(rootView);
		
		return rootView;

	}
	
	@Override 
	public void onPause() {
		super.onPause();
//		this.camcorderView.stopCamcorder();
	}
	
	@Override
	public void onStop() {
		super.onStop();
	}
	
	@Override 
	public void onDestroyView() {
		super.onDestroyView();
	}
	
	@Override
	public void onDestroy() {
		super.onDestroy();
	}
	
	@Override
	public void onDetach() {
		super.onDetach();
	}

	private void commitVideo() {
		User user = App.getUser(this);
		user.addVideoAt(videoDate, videoPath);

		// TODO: fazer isso em background
		try {
			AndroidMediaUtils.saveVideoThumbnail(thumbnailPath, videoPath);
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

	}

	void deleteVideo(String videoPath) {
		File file = new File(videoPath);
		if (file.exists()) {
			file.delete();
		}
	}
	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) {

		if (requestCode == PREVIEW_RESULT) {

			switch (resultCode) {
			case Activity.RESULT_OK:
				// mediaService.CommitVideo (videoPath);
				commitVideo();
				((MainActivity)this.getActivity()).showTimeline();
				break;

			case Activity.RESULT_CANCELED:
				deleteVideo(videoPath);
				// mediaService.RevertVideo ();
				((MainActivity)this.getActivity()).showTimeline();
				break;

			case CamcorderPreview.RetakeResult:
				deleteVideo(videoPath);
				resetCountDown();
				break;

			}
		}
	}


	void configureRecordButton(View rootView) {
		ImageButton recordButton = (ImageButton) rootView
				.findViewById(R.id.camcorderRecordButton);
		recordButton.setOnClickListener(new OnClickListener() {

			@Override
			public void onClick(View v) {
				if (!isRecording) {
					isRecording = true;
					int duration = getDuration();
					camcorderView.startRecording(videoPath, duration);
				} else {
					// TODO: implement pause
					// camcorderView.StopRecording ();
					// StartActivity (typeof(CamcorderPreview));
				}

			}
		});

	}

	void configureCountdown(View rootView) {
		countDownText = (TextView) rootView.findViewById(R.id.camcorderCountdown);

		resetCountDown();
		
		camcorderView.onSecondChange.addListener(new EventSourceListener() {
			
			@Override
			public void handleEvent(Object sender, final Object args) {
				CamcorderFragment.this.getActivity().runOnUiThread(new Runnable() {
					
					@Override
					public void run() {
						String second = (String)args;
						Log.i("YESTER", "second -> " + second);
						countDownText.setText("00:0" + second);
						
					}
				});
				
			}
		});

	}

	void configureResolutionDropdown(View rootView) {
		RelativeLayout layout = (RelativeLayout) rootView.findViewById(R.id.layoutCamcorder);

		ArrayAdapter<String> adapter = new ArrayAdapter<String>(this.getActivity(),
				android.R.layout.simple_spinner_item);
		adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
		Spinner spinner = (Spinner) rootView.findViewById(R.id.camcorderResolution);
		spinner.setAdapter(adapter);

		// spinner.ItemSelected += (s, evt) => {
		// Android.Graphics.Rect rect = new Android.Graphics.Rect ();
		// layout.GetDrawingRect (rect);
		// camcorderView.SetPreviewSize (evt.Position, rect.Width (),
		// rect.Height ());
		// };

		// camcorderView.OnCameraReady += (object sender, EventArgs e) => {
		// adapter.Clear ();
		// foreach (Camera.Size size in camcorderView.CameraSizes)
		// adapter.Add (size.Width.ToString () + " x " + size.Height.ToString
		// ());
		// };
	}

	void configureCamcorderView(View rootView) {
		RelativeLayout layout = (RelativeLayout)rootView.findViewById(R.id.layoutCamcorder);
		camcorderView = new CamcorderView(this.getActivity(), 0);
		ViewGroup.LayoutParams camcorderLayoutParams = new ViewGroup.LayoutParams(
				ViewGroup.LayoutParams.WRAP_CONTENT,
				ViewGroup.LayoutParams.WRAP_CONTENT);
		layout.addView(camcorderView, 0, camcorderLayoutParams);
		camcorderView.onVideoRecorded.addListener(new EventSourceListener() {

			@Override
			public void handleEvent(Object sender, Object args) {
				isRecording = false;
				Intent intent = new Intent(CamcorderFragment.this.getActivity(),
						CamcorderPreview.class);
				Bundle bundle = new Bundle();
				bundle.putString("video.path", videoPath);
				intent.putExtras(bundle);
				startActivityForResult(intent, PREVIEW_RESULT);

			}
		});
	}

	void configureDurationGroup(View rootView) {
		durationGroup = (RadioGroup) rootView.findViewById(R.id.camcorderGroupTime);
		durationGroup.check(R.id.camcorder3sec); // por enquanto default eh tres seg
		
		

//		 switch (userService.CurrentUser.RecordDuration) {
//		 case 1:
//		 durationGroup.Check(Resource.Id.camcorder1sec);
//		 break;
//		 case 3:
//		 durationGroup.Check (Resource.Id.camcorder3sec);
//		 break;
//		 case 5:
//		 durationGroup.Check (Resource.Id.camcorder5sec);
//		 break;
//		 }
	}

	int getDuration() {
		switch (durationGroup.getCheckedRadioButtonId()) {
		case R.id.camcorder1sec:
			return 1000;
		case R.id.camcorder3sec:
			return 3000;
		case R.id.camcorder5sec:
		default:
			return 5000;
		}

	}

	void resetCountDown() {
		countDownText.setText("00:00");
	}

	void configureServicesAndParameters() {
		// mediaService =
		// ((ISecondsApplication)this.Application).GetMediaService ();
		// userService = ((ISecondsApplication)this.Application).GetUserService
		// ();
		
		videoDate = new Date();
//		videoDate.setTime(this.getActivity().getIntent().getExtras().getLong("video.date"));
		
		ISecondsApplication app = (ISecondsApplication)getActivity().getApplication();
		
		String baseUri = app.getPathService().getMediaPath() + "/"
				+ ISecondsUtils.stringifyDate("movie", videoDate);
		
		videoPath = baseUri + ".mp4";
		thumbnailPath = baseUri + ".png";
		
//		videoPath = this.getIntent().getExtras().getString("video.path");
//		thumbnailPath = this.getIntent().getExtras().getString("thumbnail.path");
	}

	public void stopCamera() {
		this.camcorderView.stopCamcorder();
		
	}

	public void startCamera() {
		configureServicesAndParameters();
		countDownText.setText("00:00");
		this.camcorderView.startCamcorder();
		
	}

}
