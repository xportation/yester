package iSeconds.Droid;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

public class MyTimelinesFragment extends Fragment {

	private class TimelineItem {
		public String name;
		public String description;
	}
		 
	class MyTimelinesItemHolder implements ItemHolder<TimelineItem> {  
		private TextView name;  
		private TextView description;  
	    public MyTimelinesItemHolder(View base) {  
	    	name = (TextView) base.findViewById(R.id.itemMyTimelinesName);  
	    	description = (TextView) base.findViewById(R.id.itemMyTimelinesDescription);  
	    }  
	    
	    public void Update(TimelineItem item) {
	    	name.setText(item.name);
			description.setText(item.description);
	    }
	}
	
	public class MyTimelinesHolderFactory implements ItemHolderFactory<TimelineItem> {
		@Override
		public ItemHolder<TimelineItem> Build(View view) {
			return new MyTimelinesItemHolder(view);
		}		
	}
	 
	
	public MyTimelinesFragment() {
	}

	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		View rootView = inflater.inflate(R.layout.fragment_settings,
				container, false);
		return rootView;
	}
}
