package iSeconds.Droid;

import android.support.v4.app.Fragment;
import iSeconds.Domain.User;

public class App {

	public static User getUser(Fragment fragment)
	{
		ISecondsApplication app= (ISecondsApplication) fragment.getActivity().getApplication();
		return app.getUser();
	}
}
