

import static org.junit.Assert.*;

import java.util.Calendar;

import java.util.Date;
import java.util.GregorianCalendar;

import org.junit.Before;
import org.junit.BeforeClass;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.robolectric.Robolectric;
import org.robolectric.RobolectricTestRunner;
import org.robolectric.shadows.ShadowEnvironment;

import android.os.Environment;
import android.test.AndroidTestCase;
import iSeconds.Domain.DayInfo;
import iSeconds.Domain.IRepository;
import iSeconds.Domain.MediaInfo;
import iSeconds.Domain.Timeline;
import iSeconds.Domain.User;
import iSeconds.Droid.ISecondsDb;
import iSeconds.Droid.ISecondsApplication;

@RunWith(RobolectricTestRunner.class)
public class TimelineTest {

	IRepository repository = null;
	Timeline timeline = null;

	static final String dbPath = "testbase.db3";

	@BeforeClass
	public static void config() {
		ShadowEnvironment.setExternalStorageState(Environment.MEDIA_MOUNTED);
	}
	
	@Before
	public void setUp() {

		repository = ((ISecondsApplication)Robolectric.application).getRepository();

		repository.deleteAll(User.class);
		repository.deleteAll(Timeline.class);
		repository.deleteAll(DayInfo.class);
		repository.deleteAll(MediaInfo.class);

		timeline = new Timeline("my life", 1);
		timeline.save();
		timeline.setRepository(repository);
	}

	@Test
	public void testShouldBeAbleToAddAVideoInADate() {

		Date date = new GregorianCalendar(2012, 1, 1).getTime();
		timeline.addVideoAt(date, "sdcard/iseconds/video.mpeg");

		assertEquals(1, timeline.getVideosAt(date).size());
	}

	@Test
	public void TestShouldBeAbleToAddMoreThanAVideoInADate() { 
		// o usuario a principio pode adicionar mais de um video no dia.
		// na hora de "commitar" para o "trunk" do timeline ele deve escolher
		// apenas um.
		
		Date date = new GregorianCalendar(2012, 1, 1).getTime();
		
		timeline.addVideoAt(date, "sdcard/iseconds/video.mpeg");
		timeline.addVideoAt(date, "sdcard/iseconds/video2.mpeg");
		
		assertEquals(2, timeline.getVideosAt(date).size());
	}

}
