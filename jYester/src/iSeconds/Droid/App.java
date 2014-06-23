package iSeconds.Droid;

import android.app.Activity;
import android.support.v4.app.Fragment;
import iSeconds.Domain.IRepository;
import iSeconds.Domain.User;

public class App {

	public static User getUser(Fragment fragment)
	{
		ISecondsApplication app= (ISecondsApplication) fragment.getActivity().getApplication();
		return app.getUser();
	}

	public static IRepository getRepository(Activity activity) {
		ISecondsApplication app= (ISecondsApplication) activity.getApplication();
		return app.getRepository();
	}
}
