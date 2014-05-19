package iSeconds.Droid;

import java.lang.ref.WeakReference;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.AsyncTask;
import android.widget.ImageView;

public class ImageViewAsyncLoader extends AsyncTask<String, Void, Bitmap> {
	private final WeakReference<ImageView> imageViewRef;
	
	public ImageViewAsyncLoader(ImageView imageView)
	{
		this.imageViewRef= new WeakReference<ImageView>(imageView);
	}
	
	@Override
	protected Bitmap doInBackground(String... params) {
		return loadBitmap(params[0]);
	}

	private Bitmap loadBitmap(String fileName) {
		return BitmapFactory.decodeFile(fileName);
	}
	
	@Override
	protected void onPostExecute(Bitmap bitmap) {
		if (isCancelled()) {
			bitmap = null;
        }
 
        if (imageViewRef != null) {
            ImageView imageView = imageViewRef.get();
            if (imageView != null) { 
                if (bitmap != null) {
                    imageView.setImageBitmap(bitmap);
                } else {
                    imageView.setImageDrawable(imageView.getContext().getResources()
                            .getDrawable(R.drawable.ic_image_break));
                }
            }
 
        }
	}

}
