package iSeconds.Droid;

import iSeconds.Domain.IPathService;
import iSeconds.Domain.IRepository;
import android.app.Application;

import com.activeandroid.ActiveAndroid;

public class ISecondsApplication extends Application {
	
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
        repository = new ISecondsDb(this, pathService);
        repository.open();
    }

	@Override
    public void onTerminate() {
		ActiveAndroid.dispose();
		
        super.onTerminate();        
    }
}
