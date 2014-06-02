package iSeconds.Domain;

import java.util.Date;
import java.util.List;

public interface IRepository {

	void open();
	void close();
	
	<T> void deleteAll(Class<T> klass);
	<T> void saveItem(T entity);
	List<Timeline> getUserTimelines(long userId);
	void deleteTimeline(Timeline timeline);
	void saveUser(User user);
	void saveTimeline(Timeline timeline);
	DayInfo getDayInfoAt(Date date, long id);
	List<MediaInfo> getMediasForDay(DayInfo dayInfo);
	List<DayInfo> getAllDays(long id);
	List<DayInfo> getDays(long timelineId);
	User getUser(String userName);

}
