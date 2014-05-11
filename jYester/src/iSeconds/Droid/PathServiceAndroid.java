package iSeconds.Droid;

import java.io.File;

import com.activeandroid.util.Log;

import android.os.Environment;
import android.os.StatFs;
import iSeconds.Domain.IPathService;

public class PathServiceAndroid implements IPathService {

	private String appPath = "";
	private String mediaPath = "";
	private String dbPath = "";
	private String compilationPath = "";
	private boolean pathsGood = false;

	public PathServiceAndroid() {

		if (new MemoryUtils().externalMemoryAvailable()) {

			appPath = Environment.getExternalStorageDirectory().getPath()
					+ "/Yester.Droid2";
			pathsGood = true;
		}

		// System.Console.WriteLine("External Memory Available: " +
		// pathsGood.ToString());
		Log.t("Yester", "External Memory Available: " + pathsGood);

		mediaPath = appPath + "/Videos";
		dbPath = appPath + "/Db";
		compilationPath = appPath + "/Compilations";

		if (pathsGood)
			createPaths();
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
		return dbPath + "/Yester.db3";
	}

	@Override
	public String getFFMpegDbPath() {
		return dbPath + "/Yester.FFMpeg.db3";
	}

	@Override
	public String getCompilationPath() {
		return compilationPath;
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

	class MemoryUtils {
		public boolean externalMemoryAvailable() {

			String state = Environment.getExternalStorageState();
			return Environment.MEDIA_MOUNTED.equals(state);
		}

		private long availableMemorySize(File path) {
			StatFs stat = new StatFs(path.getPath());
			long blockSize = stat.getBlockSize();
			long availableBlocks = stat.getAvailableBlocks();
			return availableBlocks * blockSize;
		}

		public boolean isExternalMemoryHigherFreeSpace() {
			if (new MemoryUtils().externalMemoryAvailable())
				return availableMemorySize(Environment
						.getExternalStorageDirectory()) > availableMemorySize(Environment
						.getDataDirectory());

			return false;
		}
	}

}
