import static org.junit.Assert.*;

import java.util.List;

import iSeconds.Domain.IRepository;
import iSeconds.Domain.Timeline;
import iSeconds.Domain.User;
import iSeconds.Droid.ISecondsApplication;
import iSeconds.Droid.ISecondsDb;
import iSeconds.Droid.TimelineActivity;

import org.robolectric.Robolectric;
import org.robolectric.RobolectricTestRunner;
import org.robolectric.shadows.ShadowEnvironment;
import org.hamcrest.core.Is;
import org.junit.Before;
import org.junit.BeforeClass;
import org.junit.Test;
import org.junit.runner.RunWith;

import com.activeandroid.ActiveAndroid;

import android.os.Environment;

@RunWith(RobolectricTestRunner.class)
public class MigrationTest {
	
	ISecondsDb repository = null;

	// hardcoded for now
	static final String dbPath = "C:/Users/Aniceto/workspace/iseconds5/jYester/test/Yester.db3";
	
	@BeforeClass
	public static void config() {
		ShadowEnvironment.setExternalStorageState(Environment.MEDIA_MOUNTED);
	}
	
	@Before
	public void setup() {
		
		ActiveAndroid.dispose();
		
		repository = new ISecondsDb(Robolectric.application, dbPath);
		repository.open();
	}

	@Test
	public void testUserShouldHaveTimelines() {
		List<Timeline> timelines = repository.getUserTimelines(1);
		assertEquals(2, timelines.size());
	}
}
