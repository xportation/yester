package iSeconds.Droid;

import iSeconds.Domain.IRepository;
import iSeconds.Domain.MediaInfo;

import org.apmem.tools.layouts.FlowLayout;

import android.media.MediaPlayer;
import android.media.MediaPlayer.OnCompletionListener;
import android.os.Bundle;
import android.support.v7.app.ActionBarActivity;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.View.OnTouchListener;
import android.view.ViewGroup;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.VideoView;

public class MediaActivity extends ActionBarActivity {
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_media);

		setupVideo();				
		setupTags();
	}

	private void setupVideo() {
		long mediaId = this.getIntent().getExtras().getLong("MediaId");
		IRepository repository = App.getRepository(this);
		MediaInfo media = repository.getMedia(mediaId);
		final VideoView videoView = (VideoView) this.findViewById(R.id.mediaVideoView);
		videoView.setVideoPath(media.getVideoPath());
		videoView.seekTo(100);
		
		final ImageView imagePlay = (ImageView) this.findViewById(R.id.mediaImagePausePlay);
		videoView.setOnTouchListener(new OnTouchListener() {

			@Override
			public boolean onTouch(View view, MotionEvent motionEvent) {
				if ((motionEvent.getAction() & MotionEvent.ACTION_MASK) == MotionEvent.ACTION_DOWN)
					if (!videoView.isPlaying()) {
						imagePlay.setVisibility(View.GONE);
						videoView.seekTo(0);
						videoView.start();
						return true;
					}
				return false;
			}
		
		});
		
		videoView.setOnCompletionListener(new OnCompletionListener() {

			@Override
			public void onCompletion(MediaPlayer mediaPlayer) {
				imagePlay.setVisibility(View.VISIBLE);
			}
			
		});

	}

	private void setupTags() {
		FlowLayout layout = (FlowLayout)this.findViewById(R.id.mediaTags);
		
		LayoutInflater inflater = this.getLayoutInflater();
		
		String tags[] = { "Bagualo", "Avestruz", "Tiberio", "Bosta", "Fiz Coco", "I s2 coco" };
		for (String tag: tags) {
			View view = inflater.inflate(R.layout.closable_textview, null);
	        TextView textView = (TextView)view.findViewById(R.id.closableTextViewText);
	        textView.setText(tag);
	        layout.addView(view, 0);
		}
	}
}
