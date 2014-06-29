package iSeconds.Droid;

import java.io.File;
import java.io.IOException;

import com.activeandroid.util.Log;

import android.os.Environment;
import iSeconds.Domain.IPathService;

public class PathServiceAndroid implements IPathService {

	private String appPath = "";
	private String mediaPath = "";
	private String dbPath = "";
	private String compilationPath = "";
	private boolean pathsGood = false;
	private boolean firstExecution = false;

	public PathServiceAndroid() {

		if (MemoryUtils.externalMemoryAvailable()) {
			appPath = combine(Environment.getExternalStorageDirectory().getPath(),"Yester.Droid");
			pathsGood = true;
		}

		// System.Console.WriteLine("External Memory Available: " +
		// pathsGood.ToString());
		Log.t("Yester", "External Memory Available: " + pathsGood);

		mediaPath = combine(appPath, "Videos");
		dbPath = combine(appPath, "Db");
		compilationPath = combine(appPath, "Compilations");

		if (pathsGood) {
			firstExecution = !fileExists(getDbPath());
			createPaths();
		}
	}

	private String combine(String path1, String path2) {
		File file = new File(path1, path2);
		return file.getPath();
	}
	
	@Override
	public boolean isGood() {
		return pathsGood;
	}

	@Override
	public String getApplicationPath() {
		return appPath;
	}

	@Override
	public String getMediaPath() {
		return mediaPath;
	}

	@Override
	public String getDbPath() {
		return combine(dbPath, "YesterDroid.db3");
	}

	@Override
	public String getFFMpegDbPath() {
		return combine(dbPath, "Yester.FFMpeg.db3");
	}

	@Override
	public String getCompilationPath() {
		return compilationPath;
	}

	@Override
	public String getLegacyDbPath() {
		return combine(dbPath, "Yester.db3");
	}

	private boolean fileExists(String filename) {
		File file = new File(filename);
		return file.exists();
	}
	
	@Override
	public boolean isLegacyDb() {
		return fileExists(this.getLegacyDbPath()) && firstExecution;
	}
	
	@Override
	public void turnLegacyDbDisabled() {
		File file= new File(this.getLegacyDbPath());
		File newPath= new File(this.getLegacyDbPath() + ".disabled");
		file.renameTo(newPath);
	}
	
	void createPath(String path) {
		File file = new File(path);
		if (!file.exists()) {
			file.mkdir();
		}
	}

	void createPaths() {
		createPath(appPath);
		createPath(mediaPath);
		createPath(dbPath);
		createPath(compilationPath);
	}

	static class MemoryUtils {
		public static boolean externalMemoryAvailable() {
			String state = Environment.getExternalStorageState();
			return Environment.MEDIA_MOUNTED.equals(state);
		}
	}


}
