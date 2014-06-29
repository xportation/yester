package iSeconds.Droid;

import java.io.File;
import java.util.Date;

import iSeconds.Domain.ISecondsUtils;
import iSeconds.Domain.User;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.provider.MediaStore;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.widget.DrawerLayout;
import android.support.v7.app.ActionBar;
import android.support.v7.app.ActionBarActivity;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;

public class MainActivity extends ActionBarActivity implements
		NavigationDrawerFragment.NavigationDrawerCallbacks {

	/**
	 * Fragment managing the behaviors, interactions and presentation of the
	 * navigation drawer.
	 */
	private NavigationDrawerFragment mNavigationDrawerFragment;

	private TimelineFragment timelineFragment;

	/**
	 * Used to store the last screen title. For use in
	 * {@link #restoreActionBar()}.
	 */
	private CharSequence mTitle;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);

		mNavigationDrawerFragment = (NavigationDrawerFragment) getSupportFragmentManager()
				.findFragmentById(R.id.navigation_drawer);
		mTitle = getTitle();

		// Set up the drawer.
		mNavigationDrawerFragment.setUp(R.id.navigation_drawer,
				(DrawerLayout) findViewById(R.id.drawer_layout));
	}

	@Override
	public void onNavigationDrawerItemSelected(int index) {
		Fragment fragment = buildFragment(index);

		if (fragment != null) {
			FragmentManager fragmentManager = getSupportFragmentManager();
			fragmentManager.beginTransaction()
					.replace(R.id.container, fragment).commit();
		}

		this.onSectionAttached(index);
	}

	private Fragment buildFragment(int index) {
		Fragment fragment = null;
		switch (index) {
		case 0:
		case 1:
		case 2:
		case 3:
			timelineFragment = new TimelineFragment();
			return timelineFragment;
		case 4:
			fragment = new SettingsFragment();
			break;
		case 5:
			fragment = new AboutFragment();
			break;
		}

		return fragment;
	}

	public void onSectionAttached(int index) {
		switch (index) {
		case 0:
			mTitle = getString(R.string.session_timeline);
			break;
		case 1:
			mTitle = getString(R.string.session_range_selector);
			break;
		case 2:
			mTitle = getString(R.string.session_my_timelines);
			break;
		case 3:
			mTitle = getString(R.string.session_compilations);
			break;
		case 4:
			mTitle = getString(R.string.session_settings);
			break;
		case 5:
			mTitle = getString(R.string.session_about);
			break;
		}

		restoreActionBar();
	}

	public void restoreActionBar() {
		ActionBar actionBar = getSupportActionBar();
		actionBar.setNavigationMode(ActionBar.NAVIGATION_MODE_STANDARD);
		actionBar.setDisplayShowTitleEnabled(true);
		actionBar.setTitle(mTitle);
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu items for use in the action bar
		MenuInflater inflater = getMenuInflater();
		inflater.inflate(R.menu.timeline, menu);
		return super.onCreateOptionsMenu(menu);
	}

	static final int REQUEST_VIDEO_CAPTURE = 1;

	private Uri thumbnailUri;
	private Uri videoUri;
	private Date videoDate;

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		// Handle action bar item clicks here. The action bar will
		// automatically handle clicks on the Home/Up button, so long
		// as you specify a parent activity in AndroidManifest.xml.
		int id = item.getItemId();
		if (id == R.id.action_camera) {
			Intent takeVideoIntent = new Intent(MediaStore.ACTION_VIDEO_CAPTURE);
			if (takeVideoIntent.resolveActivity(getPackageManager()) != null) {
				takeVideoIntent.putExtra(MediaStore.EXTRA_DURATION_LIMIT, 3);

				ISecondsApplication app = (ISecondsApplication) this
						.getApplication();

				videoDate = new Date();
				
				String baseUri = app.getPathService().getMediaPath() + "/" + ISecondsUtils.stringifyDate("movie", videoDate);
				videoUri = Uri.fromFile(new File(baseUri + ".mp4"));
				thumbnailUri = Uri.fromFile(new File(baseUri + ".png"));

				takeVideoIntent.putExtra(MediaStore.EXTRA_OUTPUT, videoUri);
				takeVideoIntent.putExtra(MediaStore.EXTRA_DURATION_LIMIT, 3);
				startActivityForResult(takeVideoIntent, REQUEST_VIDEO_CAPTURE);
			}
			return true;
		}
		return super.onOptionsItemSelected(item);
	}

	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
	    if (requestCode == REQUEST_VIDEO_CAPTURE && resultCode == RESULT_OK) {
	        User user = App.getUser(this);
//	        user.addVideoAt(videoDate, videoUri.getPath());
	        
	        // TODO: fazer isso em background
	        try {
				AndroidMediaUtils.saveVideoThumbnail (thumbnailUri.getPath(), videoUri.getPath());
			} catch (Exception e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
	        
	    }
	}
}
