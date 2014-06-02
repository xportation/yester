package iSeconds.Droid;

import java.util.Date;
import java.util.List;

import com.activeandroid.ActiveAndroid;
import com.activeandroid.Configuration;
import com.activeandroid.Model;
import com.activeandroid.query.Delete;
import com.activeandroid.query.Select;

import android.content.Context;
import iSeconds.Domain.DayInfo;
import iSeconds.Domain.IRepository;
import iSeconds.Domain.MediaInfo;
import iSeconds.Domain.SqlUtils;
import iSeconds.Domain.Timeline;
import iSeconds.Domain.User;

public class ISecondsDb implements IRepository {

	private Context context = null;
	private String dbPath;

	public ISecondsDb(Context context, String dbPath) {
		this.context = context;
		this.dbPath = dbPath;
	}

	@Override
	public void open() {

		Configuration config = new Configuration.Builder(context)
				.setDatabaseName(dbPath)
				.addModelClass(User.class)
				.addModelClass(Timeline.class)
				.addModelClass(DayInfo.class)
				.addModelClass(MediaInfo.class)
				.create();
		ActiveAndroid.initialize(config);

	}

	@Override
	public void close() {
		// TODO Auto-generated method stub

	}

	@Override
	public <T> void deleteAll(Class<T> table) {

		@SuppressWarnings("unchecked")
		Class<Model> model = (Class<Model>) table;
		new Delete().from(model).execute();
	}

	@Override
	public <T> void saveItem(T entity) {
		Model model = (Model) entity;
		model.save();
	}

	@Override
	public List<Timeline> getUserTimelines(long userId) {
		// lock (locker) {
		List<Timeline> timelines = new Select().from(Timeline.class)
				.where("UserId == ?", userId).execute();
		// List<Timeline> timelines = (from i in Table<Timeline> () where
		// i.UserId == userId select i).ToList ();
		for (Timeline timeline : timelines) {
			timeline.setRepository(this);
		}
		// return (from i in Table<Timeline>() where i.UserId == userId select
		// i).ToList();
		return timelines;
		// }
	}

	@Override
	public void deleteTimeline(Timeline timeline) {
		this.deleteItem(timeline);
	}

	@Override
	public void saveTimeline(Timeline timeline) {
		this.saveItem(timeline);
	}

	@Override
	public void saveUser(User user) {
		this.saveItem(user);
	}
	
	@Override
	public User getUser(String userName) {
		User user = new Select().from(User.class)
				.where("Name == ?", userName).executeSingle();
		if (user != null)
			user.setRepository(this);
		return user;
	}

	@Override
	public DayInfo getDayInfoAt(Date date, long timelineId) {

		DayInfo dayInfo = new Select().from(DayInfo.class)
				.where("TimelineId == ?", timelineId).and("Date == ?", SqlUtils.formatToSqliteDate(date))
				.executeSingle();

		if (dayInfo == null) {
			dayInfo = new DayInfo(date, timelineId);
			dayInfo.save();
		}
		dayInfo.setRepository(this);

		return dayInfo;
	}

	@Override	
	public List<MediaInfo> getMediasForDay(DayInfo dayInfo) {
		return new Select().from(MediaInfo.class)
				.where("DayId == ?", dayInfo.getId()).execute();
	}
	
	@Override
	public List<DayInfo> getAllDays(long timelineId) {
		
		List<DayInfo> days = new Select()
			.from(DayInfo.class)
			.where("TimelineId == ?", timelineId)
			.execute();
		
		for (DayInfo day : days) {
			day.setRepository(this);
		}
		
		return days;
	}
	

	private <T extends Model> void deleteItem(T item) {
		new Delete().from(item.getClass()).where("Id == ?", item.getId())
				.execute();
	}

	@Override
	public List<DayInfo> getDays(long timelineId) {
		List<DayInfo> days= new Select().from(DayInfo.class)
				.where("TimelineId == ?", timelineId).execute();
		for (DayInfo day: days)
			day.setRepository(this);
		
		return days;
	}

}
