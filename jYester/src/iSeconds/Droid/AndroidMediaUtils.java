package iSeconds.Droid;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;

import android.graphics.Bitmap;
import android.media.ThumbnailUtils;
import android.provider.MediaStore.Video.Thumbnails;

public class AndroidMediaUtils {

	public static void saveVideoThumbnail(String thumbnailPath, String videoPath) throws Exception {
		try {
			File file = new File(thumbnailPath);
		
			if (file.exists())
				throw new Exception("Something wrong. The thumbnailPath passed already exists?");
			
			file.createNewFile();
			
			FileOutputStream fileOutput = new FileOutputStream(thumbnailPath);
			Bitmap bitmap = ThumbnailUtils.createVideoThumbnail(videoPath, Thumbnails.MICRO_KIND);
			bitmap.compress(Bitmap.CompressFormat.PNG, 100, fileOutput);
			fileOutput.flush();
			fileOutput.close();
			
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

}
