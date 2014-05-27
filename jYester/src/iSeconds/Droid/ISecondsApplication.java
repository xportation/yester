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
	
	@Override
    public void onCreate() {
        super.onCreate();
        
        pathService = new PathServiceAndroid();
        

        repository = new ISecondsDb(this, pathService.getDbPath());
        repository.open();
    }
        
        
    }
	
    @Override
    public void onTerminate() {
        super.onTerminate();
        
        repository.close();
        
        ActiveAndroid.dispose();
    }
}
