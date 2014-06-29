package iSeconds.Droid;

import android.app.Activity;
import android.support.v4.app.Fragment;
import iSeconds.Domain.IRepository;
import iSeconds.Domain.User;

public class App {

	public static User getUser(Fragment fragment)
	{
		return App.getUser(fragment.getActivity());
	}
	
	public static User getUser(Activity activity)
	{
		ISecondsApplication app= (ISecondsApplication) activity.getApplication();
		return app.getUser();
	}

	public static IRepository getRepository(Activity activity) {
		ISecondsApplication app= (ISecondsApplication) activity.getApplication();
		return app.getRepository();
	}

	public static IRepository getRepository(Fragment fragment) {
		return App.getRepository(fragment.getActivity());
	}
}
