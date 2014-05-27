

import static org.junit.Assert.*;

import org.junit.Before;
import org.junit.BeforeClass;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.robolectric.Robolectric;
import org.robolectric.RobolectricTestRunner;
import org.robolectric.shadows.ShadowEnvironment;

import iSeconds.Domain.EventSource;
import iSeconds.Domain.EventSource.EventSourceListener;
import iSeconds.Domain.IRepository;
import iSeconds.Domain.Timeline;
import iSeconds.Domain.User;
import iSeconds.Droid.ISecondsDb;
import iSeconds.Droid.ISecondsApplication;
import android.os.Environment;
import android.test.AndroidTestCase;
import android.test.mock.MockContext;

@RunWith(RobolectricTestRunner.class)
public class UserTest {
	

	User user = null;
	IRepository repository = null;

	static final String dbPath = "testbase.db3";
	
	@BeforeClass
	public static void config() {
		ShadowEnvironment.setExternalStorageState(Environment.MEDIA_MOUNTED);
	}

	@Before
	public void setUp() {

		repository = ((ISecondsApplication)Robolectric.application).getRepository();
		
		repository.deleteAll(User.class);
		user = new User("test", repository);
		repository.saveItem(user);
	}

	@Test
	public void testUserShouldHaveTimelines() {

		user.createTimeline("my life", "timeline_description");
		assertEquals(1, user.getTimelineCount());

		user.createTimeline("my daughter's life", "timeline_description");
		assertEquals(2, user.getTimelineCount());
	}

	@Test
	public void testDeleteTimeline() {
		Timeline timelineCreated = user.createTimeline("my life",
				"timeline_description");
		assertEquals(1, user.getTimelineCount());

		user.deleteTimeline(timelineCreated, false);
		assertEquals(0, user.getTimelineCount());
	}

	@Test
	public void testOnlyDeleteTimelineWithSameUserId() {
		User user2 = new User("test2", repository);
		repository.saveItem(user2);

		Timeline timelineUser1 = user.createTimeline("1", "1");
		assertEquals(1, user.getTimelineCount());

		Timeline timelineUser2 = user2.createTimeline("2", "2");
		assertEquals(1, user2.getTimelineCount());

		user.deleteTimeline(timelineUser2, false);
		user2.deleteTimeline(timelineUser1, false);

		assertEquals(1, user.getTimelineCount());
		assertEquals(1, user2.getTimelineCount());

		user.deleteTimeline(timelineUser1, false);
		user2.deleteTimeline(timelineUser2, false);

		assertEquals(0, user.getTimelineCount());
		assertEquals(0, user2.getTimelineCount());
	}

	@Test
	public void testNewTimelineWillBeCurrentOnlyIfHasNoTimelinesBefore() {
		assertEquals(0, user.getTimelineCount());
		Timeline timelineCreated = user.createTimeline("my life",
				"timeline_description");
		assertEquals(timelineCreated.getId(), user.getCurrentTimeline().getId());

		Timeline newTimelineCreated = user.createTimeline("my life2",
				"timeline_description2");
		assertEquals(timelineCreated.getId(), user.getCurrentTimeline().getId());
		assertTrue(newTimelineCreated.getId() != user.getCurrentTimeline()
				.getId());
	}

	private boolean notified = false;
	
	@Test
	public void testNotifyCurrentTimelineChangedOnlyIfTimelineWasChanged() {
		
		notified = false;
		
		user.onCurrentTimelineChanged.addListener(new EventSourceListener() {
			
			@Override
			public void handleEvent(Object sender, Object args) {
				notified = true;
			}
		}); 

		Timeline timelineCreated = user.createTimeline("my life", "timeline_description");
		assertEquals(true, notified);
		notified = false;

		user.setCurrentTimeline(timelineCreated);
		assertEquals(false, notified);

		Timeline newTimelineCreated = user.createTimeline("my life2", "timeline_description2");
		assertEquals(false, notified);

		user.setCurrentTimeline(newTimelineCreated);
		assertEquals(true, notified);
		notified = false;

		User user2 = new User("test2", repository);
		repository.saveItem(user2);
		Timeline timelineUser2 = user2.createTimeline("2", "2");

		user.setCurrentTimeline(timelineUser2);
		assertEquals(false, notified);
	}

	@Test
	public void testNotifyWhenTimelineWasUpdated() {
		notified = false;
		user.onTimelineUpdated.addListener(new EventSourceListener() {
			
			@Override
			public void handleEvent(Object sender, Object args) {
				notified = true;
			}
		});

		Timeline timelineToUpdate = user.createTimeline("my life", "timeline_description");
		assertEquals(false, notified);

		timelineToUpdate.name = "Nome lindo";
		user.updateTimeline(timelineToUpdate);
		assertEquals(true, notified);
	}
}
