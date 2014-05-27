package iSeconds.Droid.Test;

import java.util.Calendar;
import java.util.Date;
import java.util.GregorianCalendar;

import android.test.AndroidTestCase;
import iSeconds.Domain.DayInfo;
import iSeconds.Domain.MediaInfo;
import iSeconds.Domain.Timeline;
import iSeconds.Domain.User;
import iSeconds.Droid.ISecondsDb;

public class TimelineTest extends AndroidTestCase {

	ISecondsDb repository = null;
	Timeline timeline = null;

	static final String dbPath = "testbase.db3";

	@Override
	public void setUp() {

		repository = new ISecondsDb(getContext(), dbPath);
		repository.open();

		repository.deleteAll(User.class);
		repository.deleteAll(Timeline.class);
		repository.deleteAll(DayInfo.class);
		repository.deleteAll(MediaInfo.class);

		timeline = new Timeline("my life", 1);
		timeline.save();
		timeline.setRepository(repository);
	}

	public void testShouldBeAbleToAddAVideoInADate() {

		Date date = new GregorianCalendar(2012, 1, 1).getTime();
		timeline.addVideoAt(date, "sdcard/iseconds/video.mpeg");

		assertEquals(1, timeline.getVideosAt(date).size());
	}

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
