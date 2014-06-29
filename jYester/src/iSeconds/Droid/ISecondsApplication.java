package iSeconds.Droid;

import iSeconds.Domain.IPathService;
import iSeconds.Domain.IRepository;
import iSeconds.Domain.User;
import android.app.Application;

import com.activeandroid.ActiveAndroid;

public class ISecondsApplication extends Application {
	
	private User user = null;
	private IPathService pathService = null;
	private IRepository repository = null;
	
	public IRepository getRepository() {
		return repository;
	}
	
	public IPathService getPathService() {
		return pathService;
	}
	
	@Override
    public void onCreate() {
        super.onCreate();
        
        pathService = new PathServiceAndroid();
//        repository = new ISecondsDb(this, pathService.getLegacyDbPath());
        repository = new ISecondsDb(this, pathService);
        repository.open();
        
//        login();
    }
	
	public User getUser() {
		return user;
	}
	
//    private void login() {
//		user= repository.getUser("user");
//		if (user == null) {
//			user= new User("user", repository);
//			repository.saveUser(user);			
//			user.createTimeline(this.getString(R.string.default_timeline_name), 
//					this.getString(R.string.default_timeline_description));
//		}
//	}

	@Override
    public void onTerminate() {
        super.onTerminate();
        
//        repository.close();
        
        ActiveAndroid.dispose();
    }
}
