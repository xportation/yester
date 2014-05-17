package iSeconds.Droid;

import java.util.List;

import com.activeandroid.ActiveAndroid;
import com.activeandroid.Configuration;
import com.activeandroid.Model;
import com.activeandroid.query.Delete;
import com.activeandroid.query.Select;

import android.content.Context;
import iSeconds.Domain.IRepository;
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
		
//        String path = Environment.getExternalStorageDirectory().getAbsolutePath() + "/testDb";
//        File folder = new File(path);
//        boolean result = false;
//        if (!folder.exists()) 
//        	result = folder.mkdirs();
        	
        Configuration config = new Configuration.Builder(context).setDatabaseName(dbPath).create();
        ActiveAndroid.initialize(config);
		
	}

	@Override
	public void close() {
		// TODO Auto-generated method stub
		
	}

	@Override
	public <T> void deleteAll(Class<T> table) {
		
		@SuppressWarnings("unchecked")
		Class<Model> model = (Class<Model>)table;
		new Delete().from(model).execute();		
	}

	@Override
	public <T> void saveItem(T entity) {
		Model model = (Model)entity;
		model.save();
	}

	@Override
	public List<Timeline> getUserTimelines(long userId) {
//		lock (locker) {
			List<Timeline> timelines = new Select().from(Timeline.class).where("UserId == ?", userId).execute();
//			List<Timeline> timelines = (from i in Table<Timeline> () where i.UserId == userId select i).ToList ();
			for (Timeline timeline : timelines) {
				timeline.setRepository (this);
			}
			//return (from i in Table<Timeline>() where i.UserId == userId select i).ToList();
			return timelines;
//		}
	}
	
	@Override
	public void deleteTimeline(Timeline timeline) {
		this.deleteItem(timeline);
	}
	
	@Override
	public void saveUser(User user) {
		this.saveItem(user);
	}
	
	private <T extends Model> void deleteItem(T item) {
		new Delete().from(item.getClass()).where("Id == ?", item.getId()).execute();
	}


}
