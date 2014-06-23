package iSeconds.Droid;

import android.os.Bundle;
import android.support.v7.app.ActionBarActivity;
import android.widget.ImageView;
import android.widget.VideoView;

public class PlayerActivity extends ActionBarActivity {
	private boolean usesController= false;
	private ImageView playOverImage= null;

	private VideoView videoView;
	private int lastVideoPosition = 0;
	private boolean startingPaused = false;

	private final String VideoPaused= "videoPaused";
	private final String VideoPosition= "videoPosition";
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_player);
				
		String fileName = this.getIntent().getExtras().getString("FileName");
		videoView = (VideoView) this.findViewById(R.id.playerVideoView);
		videoView.setVideoPath(fileName);
		videoView.start();
	}
}
