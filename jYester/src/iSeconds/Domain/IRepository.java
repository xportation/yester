package iSeconds.Domain;

import java.util.List;

public interface IRepository {

	void open();
	void close();
	
	<T> void deleteAll(Class<T> klass);
	<T> void saveItem(T entity);
	List<Timeline> getUserTimelines(long userId);
	void deleteTimeline(Timeline timeline);
	void saveUser(User user);

}
