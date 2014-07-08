package iSeconds.Droid;

import uk.co.senab.bitmapcache.BitmapLruCache;
import android.app.Activity;
import android.support.v4.app.Fragment;
import iSeconds.Domain.IRepository;

public class App {

	public static IRepository getRepository(Activity activity) {
		ISecondsApplication app= (ISecondsApplication) activity.getApplication();
		return app.getRepository();
	}

	public static IRepository getRepository(Fragment fragment) {
		return App.getRepository(fragment.getActivity());
	}
	
	public static BitmapLruCache getBitmapCache(Fragment fragment) {
		ISecondsApplication app= (ISecondsApplication) fragment.getActivity().getApplication();
		return app.getBitmapCache();
	}
}
