package iSeconds.Droid;

import java.lang.ref.WeakReference;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Color;
import android.graphics.drawable.ColorDrawable;
import android.graphics.drawable.Drawable;
import android.os.AsyncTask;
import android.widget.ImageView;

public class AsyncImageLoader 
{
	private BitmapLoaderTask task = null;
	
    public void load(String path, ImageView imageView) {
    	task = new BitmapLoaderTask(imageView);
        LoadedDrawable downloadedDrawable = new LoadedDrawable(task);
        imageView.setImageDrawable(downloadedDrawable);
        task.execute(path);
    }
	
	
	private static BitmapLoaderTask getBitmapLoaderTask(ImageView imageView) {
        if (imageView != null) {
            Drawable drawable = imageView.getDrawable();
            if (drawable instanceof LoadedDrawable) {
                LoadedDrawable loadedDrawable = (LoadedDrawable)drawable;
                return loadedDrawable.getBitmapLoaderTask();
            }
        }
        return null;
    }

    private Bitmap loadBitmap(String path) {		
		return BitmapFactory.decodeFile(path);
	}

	class BitmapLoaderTask extends AsyncTask<String, Void, Bitmap> {
		private String path;
        private final WeakReference<ImageView> imageViewReference;
               
        public BitmapLoaderTask(ImageView imageView) {
            imageViewReference = new WeakReference<ImageView>(imageView);
        }

        @Override
        protected Bitmap doInBackground(String... params) {
            this.path = params[0];
            return loadBitmap(this.path);
        }

		@Override
        protected void onPostExecute(Bitmap bitmap) {
            if (isCancelled()) {
                bitmap = null;
            }

            if (imageViewReference != null) {
                ImageView imageView = imageViewReference.get();
                BitmapLoaderTask bitmapLoaderTask = getBitmapLoaderTask(imageView);
                // Change bitmap only if this process is still associated with it
                // Or if we don't use any bitmap to task association (NO_DOWNLOADED_DRAWABLE mode)
                if (this == bitmapLoaderTask) {
                    imageView.setImageBitmap(bitmap);
                }
            }
        }	
	}
	
	//TODO Usar cor em color.xml para ser a cor de fundo
    static class LoadedDrawable extends ColorDrawable {
        private final WeakReference<BitmapLoaderTask> bitmapLoaderTaskReference;

        public LoadedDrawable(BitmapLoaderTask bitmapLoaderTask) {
            super(Color.WHITE);
            bitmapLoaderTaskReference =
                new WeakReference<BitmapLoaderTask>(bitmapLoaderTask);
        }

        public BitmapLoaderTask getBitmapLoaderTask() {
            return bitmapLoaderTaskReference.get();
        }
    }
}
