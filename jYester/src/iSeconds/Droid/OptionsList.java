package iSeconds.Droid;

import java.util.ArrayList;
import java.util.List;

public class OptionsList 
{
	private IOptionCallback callback = null;
	private List<String> entries = new ArrayList<String>();
	
	public OptionsList(IOptionCallback callback) {
		this.callback = callback;
	}
	
	public void add(String name) {
		entries.add(name);
	}

	public CharSequence[] ToItems() {
		CharSequence[] items = new CharSequence[entries.size()];
		
		int index= 0;
		for (String entry: entries) {
			items[index] = entry;
			index++;
		}
					
		return items;
	}

	public void invoke(int which) {
		callback.invoke(which);
	}
}
