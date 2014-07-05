package iSeconds.Droid;

import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.VideoView;

public class CamcorderPreview extends Activity {
	
	public static int PreviewResult = 1;
	
	public static final int RetakeResult = Activity.RESULT_FIRST_USER + 1; 

	@Override
	protected void onCreate (Bundle bundle)
	{
		super.onCreate (bundle);

		String videoPath = this.getIntent().getExtras().getString ("video.path");

		setContentView(R.layout.fragment_camcorder_preview);
		VideoView videoView = (VideoView) findViewById (R.id.camcorderPreviewVideo);
		videoView.setVideoPath (videoPath);
		videoView.start ();

		Button confirmButton = (Button)this.findViewById (R.id.camcorderPreviewConfirmButton);
		
		confirmButton.setOnClickListener(new OnClickListener() {
			
			@Override
			public void onClick(View v) {
				CamcorderPreview.this.setResult(Activity.RESULT_OK);
				CamcorderPreview.this.finish();
				
			}
		});
		
		Button retakeButton = (Button)this.findViewById (R.id.camcorderPreviewRetakeButton);
		retakeButton.setOnClickListener(new OnClickListener() {
			
			@Override
			public void onClick(View v) {
				CamcorderPreview.this.setResult(RetakeResult);
				CamcorderPreview.this.finish();

			}
		});

		Button cancelButton = (Button)this.findViewById (R.id.camcorderPreviewCancelButton);
		cancelButton.setOnClickListener(new OnClickListener() {
			
			@Override
			public void onClick(View v) {
				CamcorderPreview.this.setResult(Activity.RESULT_CANCELED);
				CamcorderPreview.this.finish();
				
			}
		});

	}

}
