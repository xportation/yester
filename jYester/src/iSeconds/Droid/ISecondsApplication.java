package iSeconds.Droid;

import iSeconds.Domain.IPathService;
import iSeconds.Domain.IRepository;

import java.io.File;

import android.app.Application;
import android.content.Context;
import android.content.ContextWrapper;
import android.database.sqlite.SQLiteDatabase;
import android.os.Environment;

import com.activeandroid.ActiveAndroid;
import com.activeandroid.Configuration;

public class ISecondsApplication extends Application {
	
	
	private IPathService pathService = null;
	private IRepository repository = null;
	
	@Override
    public void onCreate() {
        super.onCreate();
        
        pathService = new PathServiceAndroid();
        repository = new ISecondsDb(this, pathService.getDbPath());
        repository.open();
        
    }
    @Override
    public void onTerminate() {
        super.onTerminate();
        
        repository.close();
        
        ActiveAndroid.dispose();
    }
}
