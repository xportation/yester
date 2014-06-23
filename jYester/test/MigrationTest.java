import static org.junit.Assert.*;

import java.util.Date;
import java.util.GregorianCalendar;
import java.util.List;

import iSeconds.Domain.DayInfo;
import iSeconds.Domain.IRepository;
import iSeconds.Domain.MediaInfo;
import iSeconds.Domain.SqlUtils;
import iSeconds.Domain.Timeline;
import iSeconds.Domain.User;
import iSeconds.Droid.ISecondsApplication;
import iSeconds.Droid.ISecondsDb;
import iSeconds.Droid.MainActivity;

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
		
		Timeline timeline1 = timelines.get(0);
		Timeline timeline2 = timelines.get(1);
		
		assertEquals("Default Timeline", timeline1.name);
		assertEquals("Teste", timeline2.name);
		
		assertEquals(104, timeline1.getDays().size());
		assertEquals(0, timeline2.getDays().size());
	}
	
	@Test
	public void testAssertTimelineDays() throws Exception {
		
		Timeline timeline = repository.getUserTimelines(1).get(0);
		
		List<DayInfo> days = timeline.getDays();
		
		DayInfo day = days.get(0);
		assertEquals(new GregorianCalendar(2013, 8, 8).getTime(), day.getDate());
		List<MediaInfo> videos = day.getVideos();
		assertEquals(1, videos.size());
		
	}
}
