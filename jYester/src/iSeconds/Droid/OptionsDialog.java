package iSeconds.Droid;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;

public class OptionsDialog {

	public static void ShowDialog(Activity context, final OptionsList options) {
		AlertDialog.Builder builder = new AlertDialog.Builder(context);
		builder.setItems(options.ToItems(), new DialogInterface.OnClickListener() {

			@Override
			public void onClick(DialogInterface dialog, int which) {
				options.invoke(which);	
			}
		
		});
		
		builder.create().show();
	}
}
