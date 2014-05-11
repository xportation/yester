package iSeconds.Droid;

import java.io.File;

import com.activeandroid.ActiveAndroid;
import com.activeandroid.Configuration;

import android.content.Context;
import android.os.Environment;
import iSeconds.Domain.IPathService;
import iSeconds.Domain.IRepository;

public class ISecondsDb implements IRepository {
	
	private static final String databaseFile = "Yester.db";
	
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

}
