package iSeconds.Droid;

import uk.co.senab.bitmapcache.BitmapLruCache;
import iSeconds.Domain.IPathService;
import iSeconds.Domain.IRepository;
import android.app.Application;

import com.activeandroid.ActiveAndroid;

public class ISecondsApplication extends Application {

	private BitmapLruCache bitmapCache;
	private IRepository repository = null;
	private IPathService pathService = null;
	
	public IRepository getRepository() {
		return repository;
	}
	
	public IPathService getPathService() {
		return pathService;
	}
	
	public BitmapLruCache getBitmapCache() {
		return bitmapCache;
	}
	
	@Override
    public void onCreate() {
        super.onCreate();
        
        BitmapLruCache.Builder bitmapCacheBuilder = new BitmapLruCache.Builder(this);
        bitmapCacheBuilder.setMemoryCacheEnabled(true).setMemoryCacheMaxSizeUsingHeapSize(50f);
		bitmapCache = bitmapCacheBuilder.build();
        
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
