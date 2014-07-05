package iSeconds.Droid;

import iSeconds.Domain.Media;

import org.apmem.tools.layouts.FlowLayout;

import android.media.MediaPlayer;
import android.media.MediaPlayer.OnCompletionListener;
import android.os.Bundle;
import android.support.v7.app.ActionBarActivity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.ImageButton;
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
		Media media = Media.load(Media.class, mediaId);
		final VideoView videoView = (VideoView) this.findViewById(R.id.mediaVideoView);
		videoView.setVideoPath(media.getVideoPath());
		videoView.seekTo(100);
		
		final ImageButton imagePlay = (ImageButton) this.findViewById(R.id.mediaPlayButton);
		imagePlay.setOnClickListener(new OnClickListener() {

			@Override
			public void onClick(View view) {
				if (!videoView.isPlaying()) {
					imagePlay.setVisibility(View.GONE);
					videoView.seekTo(0);
					videoView.start();
				}
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
			View view = inflater.inflate(R.layout.item_closable_textview, null);
	        TextView textView = (TextView)view.findViewById(R.id.closableTextViewText);
	        textView.setText(tag);
	        layout.addView(view, 0);
		}
	}
}
