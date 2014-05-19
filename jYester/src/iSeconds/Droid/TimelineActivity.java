package iSeconds.Droid;

import android.app.Activity;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.widget.DrawerLayout;
import android.support.v7.app.ActionBar;
import android.support.v7.app.ActionBarActivity;
import android.view.LayoutInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

public class TimelineActivity extends ActionBarActivity implements
		NavigationDrawerFragment.NavigationDrawerCallbacks {

	/**
	 * Fragment managing the behaviors, interactions and presentation of the
	 * navigation drawer.
	 */
	private NavigationDrawerFragment mNavigationDrawerFragment;

	/**
	 * Used to store the last screen title. For use in
	 * {@link #restoreActionBar()}.
	 */
	private CharSequence mTitle;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_timeline);

		mNavigationDrawerFragment = (NavigationDrawerFragment) getSupportFragmentManager()
				.findFragmentById(R.id.navigation_drawer);
		mTitle = getTitle();

		// Set up the drawer.
		mNavigationDrawerFragment.setUp(R.id.navigation_drawer,
				(DrawerLayout) findViewById(R.id.drawer_layout));
	}

	@Override
	public void onNavigationDrawerItemSelected(int index) {
		Fragment fragment= buildFragment(index);
		
		if (fragment != null) {
			FragmentManager fragmentManager = getSupportFragmentManager();
			fragmentManager
					.beginTransaction()
					.replace(R.id.container,fragment)
					.commit();
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
			fragment= new TimelineFragment();
			break;
		case 4:
			fragment= new SettingsFragment();
			break;
		case 5:
			fragment= new AboutFragment();
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
	public boolean onOptionsItemSelected(MenuItem item) {
		// Handle action bar item clicks here. The action bar will
		// automatically handle clicks on the Home/Up button, so long
		// as you specify a parent activity in AndroidManifest.xml.
//		int id = item.getItemId();
//		if (id == R.id.action_settings) {
//			return true;
//		}
		return super.onOptionsItemSelected(item);
	}

}
